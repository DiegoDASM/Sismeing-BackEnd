# Despliegue de SISMEING en un servidor Linux (stack Bitnami)

Frontend y API se sirven bajo **el mismo dominio** mediante nginx. Es a propósito:
al ser mismo origen el navegador no dispara CORS, así que no hay que configurar
`Cors:AllowedOrigins` ni mantener dos dominios sincronizados.

```
Internet ──► nginx :443 ──┬─► /        archivos estáticos en /opt/sismeing/web
                          └─► /api/    proxy a Kestrel en 127.0.0.1:5080
```

Kestrel escucha **solo en localhost**: el puerto 5080 nunca queda expuesto.

---

## 1. Preparar los artefactos (en tu máquina Windows)

```powershell
cd c:\Users\Paul\Documents\github\tesis\Sismeing-BackEnd
dotnet publish Sismeing.API\Sismeing.API.csproj -c Release -r linux-x64 --self-contained false -o publish

cd ..\frontsismeing
npm ci
npm run build      # genera dist/
```

`appsettings.Development.json` está excluido de la publicación (ver `Sismeing.API.csproj`),
así que **los secretos no viajan al servidor**. Verifícalo antes de subir:

```powershell
dir publish\appsettings*.json     # debe salir solo appsettings.json
```

## 2. Instalar el runtime en el servidor

Bitnami no trae stack de .NET, hay que instalarlo. En Debian/Ubuntu:

```bash
wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O /tmp/ms.deb
sudo dpkg -i /tmp/ms.deb
sudo apt-get update && sudo apt-get install -y aspnetcore-runtime-9.0
dotnet --list-runtimes        # debe aparecer Microsoft.AspNetCore.App 9.0.x
```

> Basta el **runtime**, no el SDK: la compilación ya la hiciste en Windows.

## 3. Crear usuario y carpetas

```bash
sudo useradd --system --no-create-home --shell /usr/sbin/nologin sismeing
sudo mkdir -p /opt/sismeing/api /opt/sismeing/web /etc/sismeing
```

## 4. Subir los archivos

Desde tu máquina (ajusta usuario y host):

```powershell
scp -r publish/*              usuario@TU-SERVIDOR:/tmp/api/
scp -r ..\frontsismeing\dist\* usuario@TU-SERVIDOR:/tmp/web/
```

En el servidor:

```bash
sudo cp -r /tmp/api/* /opt/sismeing/api/
sudo cp -r /tmp/web/* /opt/sismeing/web/
sudo chown -R sismeing:sismeing /opt/sismeing/api
sudo chown -R www-data:www-data /opt/sismeing/web    # o bitnami:daemon en Bitnami
```

## 5. Configurar los secretos

```bash
sudo cp api.env.example /etc/sismeing/api.env
sudo nano /etc/sismeing/api.env      # rellenar valores reales
sudo chmod 600 /etc/sismeing/api.env
sudo chown root:root /etc/sismeing/api.env
```

La `JwtSettings__SecretKey` es la clave nueva que está en tu
`appsettings.Development.json` local. **No reutilices la vieja**: está en el
historial de git y permite falsificar tokens de SuperAdmin.

## 6. Levantar el servicio

```bash
sudo cp sismeing-api.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable --now sismeing-api
sudo systemctl status sismeing-api
```

Comprobar que responde en local antes de tocar nginx:

```bash
curl http://127.0.0.1:5080/api/health
# {"estado":"OK","version":"1.0.0","timestamp":"..."}
```

Si falla: `journalctl -u sismeing-api -n 50 --no-pager`

## 7. Configurar nginx

```bash
sudo cp nginx-sismeing.conf /opt/bitnami/nginx/conf/server_blocks/sismeing-server-block.conf
sudo nano /opt/bitnami/nginx/conf/server_blocks/sismeing-server-block.conf   # poner TU-DOMINIO.com
sudo /opt/bitnami/nginx/sbin/nginx -t          # validar sintaxis
sudo /opt/bitnami/ctlscript.sh restart nginx
```

> En una instalación de nginx estándar (no Bitnami) la ruta es
> `/etc/nginx/sites-available/` + enlace en `sites-enabled/`, y se recarga con
> `sudo systemctl reload nginx`.

## 8. HTTPS

```bash
sudo /opt/bitnami/bncert-tool          # herramienta de Bitnami, lo hace todo
```

Alternativa genérica: `sudo certbot --nginx -d TU-DOMINIO.com`

Certbot reescribe el bloque de servidor para escuchar en 443 y redirigir el 80.

## 9. Verificar

```bash
curl -I  https://TU-DOMINIO.com/                 # 200, sirve el frontend
curl     https://TU-DOMINIO.com/api/health       # {"estado":"OK",...}
```

En el navegador: entra, inicia sesión y **recarga estando en `/dashboard`**.
Si carga bien, el `try_files` funciona; si da 404, revisa el bloque `location /`.

---

## Actualizaciones posteriores

```bash
# API
sudo systemctl stop sismeing-api
sudo cp -r /tmp/api/* /opt/sismeing/api/
sudo chown -R sismeing:sismeing /opt/sismeing/api
sudo systemctl start sismeing-api

# Frontend (no requiere parar nada)
sudo cp -r /tmp/web/* /opt/sismeing/web/
```

## Diagnóstico

| Síntoma | Dónde mirar |
|---|---|
| 502 Bad Gateway | El servicio está caído: `systemctl status sismeing-api` |
| 404 al recargar en `/dashboard` | Falta el `try_files` del bloque `location /` |
| Los informes fallan | Debe existir `/opt/sismeing/api/refs/`; RazorLight la necesita |
| No conecta a la base | `journalctl -u sismeing-api`; revisa la cadena en `/etc/sismeing/api.env` |
| Fotos rechazadas | Sube `client_max_body_size` en nginx |
| La auditoría registra siempre la misma IP | Falta `X-Forwarded-For` en el bloque `location /api/` |
