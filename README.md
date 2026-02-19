# VideoPlayer — Reproductor Minimalista (C# WinForms)

Reproductor de video con diseño **negro + azul claro**, controles clásicos
y panel de propiedades del video. Motor de reproducción: **LibVLCSharp**.

---

## Archivos del proyecto

| Archivo | Descripción |
|---|---|
| `Program.cs` | Punto de entrada |
| `Form1.cs` | Lógica principal del reproductor |
| `Form1.Designer.cs` | Layout e instanciación de controles |
| `Theme.cs` | Paleta de colores y fuentes |
| `VideoFileInfo.cs` | Modelo de datos / propiedades del video |
| `FlatButton.cs` | Botón personalizado con tema oscuro |
| `SeekBar.cs` | Barra seek con diseño personalizado |
| `VolumeBar.cs` | Control de volumen compacto |
| `VideoPlayer.csproj` | Archivo de proyecto MSBuild |

---

## Requisitos

- **Visual Studio 2022** (o VS Code + SDK .NET 8)
- **.NET 8 SDK** (`net8.0-windows`)
- Paquetes **NuGet** (se instalan automáticamente con Restore):

```
LibVLCSharp            >= 3.9
VideoLAN.LibVLC.Windows >= 3.0
LibVLCSharp.WinForms   >= 3.9
```

> ⚠️ El proyecto **debe compilarse como x64** (ya está configurado en el `.csproj`).
> Verificar en: Proyecto → Propiedades → Compilar → Destino de plataforma = `x64`

---

## Cómo abrir y ejecutar

1. Crea una nueva carpeta, por ejemplo `VideoPlayer/`
2. Copia todos los archivos `.cs` y el `.csproj` dentro
3. Abre `VideoPlayer.csproj` con Visual Studio 2022
4. Haz clic derecho en la solución → **Restaurar paquetes NuGet**
5. Compila y ejecuta (`F5` o `Ctrl+F5`)

Desde línea de comandos:
```bash
cd VideoPlayer
dotnet restore
dotnet run
```

---

## Controles de teclado

| Tecla | Acción |
|---|---|
| `Space` | Reproducir / Pausar |
| `←` / `→` | Retroceder / Avanzar 5 s |
| `↑` / `↓` | Subir / Bajar volumen |
| `M` | Silenciar / Activar audio |
| `F11` | Pantalla completa |
| `Esc` | Salir de pantalla completa |
| `Enter` (en lista) | Reproducir ítem seleccionado |
| `Del` (en lista) | Quitar ítem de la lista |

---

## Funcionalidades

- ▶ Reproducir / ⏸ Pausar / ■ Detener
- ⏮ Anterior · ⏭ Siguiente
- 🔊 Volumen con mute
- Barra de progreso seekable con arrastre
- Velocidad de reproducción: 0.25× – 3×
- 🔀 Aleatorio · 🔁 Repetir
- ⛶ Pantalla completa (F11 / Escape)
- **Arrastrar y soltar** archivos de video directamente al reproductor
- Lista de reproducción (ListView) con renumeración automática
- Panel lateral de **propiedades del video**:
  - Nombre, duración, tamaño, formato
  - Resolución, FPS, códec video
  - Códec audio, frecuencia de muestreo, canales
- Formatos soportados: `.mp4`, `.avi`, `.mkv`, `.mov`, `.wmv`, `.flv`, `.webm`, `.m4v`, `.ts`, `.mpg`
