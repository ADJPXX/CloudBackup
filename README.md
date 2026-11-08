# CloudBackup

Aplicativo de backup para Windows que copia pastas pessoais, arquivos de configuração de jogos e dados de alguns aplicativos para um disco local. Em seguida, pode copiar pastas selecionadas para uma pasta sincronizada por um serviço de nuvem, como um Google Drive montado no Windows.

O projeto é específico para o ambiente em que foi criado: alguns caminhos e unidades estão fixos no código (`D:`, `E:` e, por padrão, `G:`). Ajuste a configuração e os caminhos antes de usar em outro computador.

## O que ele faz

O fluxo de execução em `Program.Main` é:

1. Verifica se o processo está elevado como Administrador. Se não estiver, solicita elevação pelo UAC e encerra o processo atual.
2. Lê `CloudBackupConfig.json` ao lado do executável; se o arquivo não existir, cria um com valores iniciais definidos no código.
3. Garante a existência de `CloudBackupLog.log` ao lado do executável.
4. Executa as cópias para o disco local.
5. Só se a rotina local retornar sucesso, executa a etapa de cópia para a pasta de nuvem configurada.

As cópias são feitas pelo `robocopy` do Windows. Não há integração com API de nuvem: a etapa chamada “nuvem” apenas copia para uma pasta local/montada que outro aplicativo pode sincronizar.

## Conteúdo copiado

### Pastas de Documentos configuráveis

`BackupFolders` contém nomes de pastas dentro de `Meus Documentos`. Cada pasta encontrada é copiada para `D:\Backups\<nome>`. Na configuração inicial, a lista inclui `AC Content Manager`, `Assetto Corsa`, `Assetto Corsa Competizione`, `Automobilista 2`, `iRacing`, `My Games`, `PCSX2` e `RaceLabApps`.

`My Games` é um caso especial: o programa copia somente `Documentos\My Games\Rocket League` para `D:\Backups\My Games\Rocket League`. Outros conteúdos de `My Games` não são copiados por esse fluxo.

### Cópias adicionais para o disco local

| Origem | Destino | Observações |
| --- | --- | --- |
| `Documentos\SCRIPTS` | `D:\SCRIPTS` | Cópia recursiva. |
| Subpastas imediatas de `E:\Repositories` | `D:\Codigos\<nome>` | Cada diretório filho é copiado. |
| `E:\Repositories\C#\.github` e o arquivo `.gitignore` de `E:\Repositories\C#` | `D:\Codigos\C#` | Cópias adicionais feitas pela rotina de repositórios. |
| `%APPDATA%\Blackmagic Design` | `D:\Backups\DaVinci Resolve\Blackmagic Design` | Exclui diretórios com nomes listados em `ExcludedFolders`. |
| `%APPDATA%\obs-studio` | `D:\Backups\obs-studio` | Exclui diretórios configurados. |
| `%LOCALAPPDATA%\DuckStation` | `D:\Backups\DuckStation` | Exclui diretórios configurados. |
| Pasta `cfg` do Counter-Strike em `Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\game\csgo` | `D:\Backups\CS\CFGS` | Apenas arquivos `*.cfg`. |
| `%LOCALAPPDATA%\iRacing\_useSteam.txt` | `D:\Backups\iRacingLocal` | Apenas o arquivo `_useSteam.txt`. |
| Pasta `UserData` do Le Mans Ultimate em `Program Files (x86)\Steam\steamapps\common\Le Mans Ultimate` | `D:\Backups\LMU\UserData` | Exclui diretórios configurados. |
| `%APPDATA%\racelabapps` | `D:\Backups\RaceLabAppData\racelabapps` | Apenas arquivos `*.json`. |
| `%APPDATA%\Battlestate Games\Escape from Tarkov\Settings` | `D:\Backups\Tarkov` | Apenas arquivos `*.ini`. |
| `Program Files (x86)\MSI Afterburner\Profiles` | `D:\Backups\Afterburner\Profiles` | Exclui diretórios configurados. |
| `Program Files (x86)\RivaTuner Statistics Server\Profiles` e `Plugins` | `D:\Backups\Riva\Profiles` e `D:\Backups\Riva\Plugins` | Exclui diretórios configurados. |
| `%USERPROFILE%\Downloads\TUDO` | `D:\Backups\TUDO` | Cópia recursiva. |

As pastas ausentes são registradas e ignoradas. Os caminhos de aplicativos são montados usando as pastas conhecidas do Windows, mas as letras `D:` e `E:` e os caminhos específicos do Steam permanecem definidos no código.

### Cópia para a pasta de nuvem

`CloudBackupFolders` lista nomes de pastas que o programa procura diretamente na raiz de `D:`. Cada pasta existente é copiada para `<CloudBackupPath>\<nome>`. A configuração inicial usa `G:\Meu Drive\BackupCloud\` como destino e inclui `Backups`, `Book do globis`, `Codigos`, `Contratos apartamentos`, `Fotos Steam`, `Instaladores`, `Jogos e emuladores`, `SCRIPTS`, `Vídeos` e `Wallpapers`.

Se o destino configurado ainda não estiver disponível, o programa verifica novamente a cada 18 segundos, por até 10 tentativas (cerca de 3 minutos). Se continuar indisponível, registra que a cópia para a nuvem não foi feita.

## Configuração

O arquivo `CloudBackupConfig.json` é criado junto do executável na primeira execução. As propriedades aceitas são:

```json
{
  "BackupFolders": [
    "Assetto Corsa",
    "My Games"
  ],
  "CloudBackupFolders": [
    "Backups",
    "Codigos",
    "SCRIPTS"
  ],
  "CloudBackupPath": "G:\\Meu Drive\\BackupCloud\\",
  "ExcludedFolders": [
    "log",
    "cache",
    "replay",
    "logs",
    "caches",
    "replays"
  ]
}
```

- `BackupFolders`: nomes das pastas a procurar em `Meus Documentos`.
- `CloudBackupFolders`: nomes das pastas a procurar diretamente em `D:` para a segunda etapa.
- `CloudBackupPath`: pasta de destino da etapa de nuvem.
- `ExcludedFolders`: nomes de diretórios excluídos nas cópias que passam essa lista ao `robocopy`; não se aplica a todos os fluxos de cópia.

A configuração não altera os caminhos fixos das cópias adicionais. Para mudar essas origens ou destinos, edite `Services/PathsServices.cs` e, quando necessário, os fluxos em `Services/DriveBackupService.cs`.

## Requisitos

- Windows, com `robocopy` disponível (incluído no Windows).
- .NET 10 SDK para compilar a partir do código.
- Acesso de Administrador, solicitado pelo próprio aplicativo.
- Unidades e pastas configuradas existentes e acessíveis. O destino local padrão é `D:`; os repositórios são lidos de `E:\Repositories`; o destino de nuvem inicial é `G:`.

## Compilar e publicar

Na pasta do repositório:

```powershell
dotnet restore
dotnet build .\CloudBackup.sln -c Release
```

Para gerar o executável Windows x64 independente, em arquivo único:

```powershell
dotnet publish .\CloudBackup.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  /p:PublishSingleFile=true `
  /p:PublishTrimmed=false `
  -o publish
```

O fluxo em `.github/workflows/release.yml` publica automaticamente quando há push para `master`. Ele gera um release `build-<número da execução>` com `CloudBackup.exe` para Windows x64.

## Logs e códigos do Robocopy

O log é gravado em `CloudBackupLog.log` ao lado do executável. O serviço interpreta os códigos de saída do `robocopy`: de 0 a 7 representam combinações de cópia, arquivos extras ou incompatíveis; 8 ou mais indicam falha em algum nível.

## Limitações conhecidas do comportamento atual

- Se `CloudBackupConfig.json` for inválido ou ilegível, o erro é capturado e a execução continua com a configuração vazia em memória. Isso pode pular as pastas configuradas e, como o caminho da nuvem fica vazio, levar à espera de até 3 minutos antes de desistir dessa etapa.
- As cópias usam `/E`, não `/MIR`: incluem subpastas, inclusive vazias, mas não removem do destino arquivos que foram removidos da origem. Isso não cria versões históricas nem snapshots.
- O programa não implementa restauração, retenção, compressão, criptografia ou verificação independente de integridade.
- Códigos de saída do `robocopy` e exceções dentro de algumas rotinas são registrados como texto, mas não são convertidos consistentemente em falha da etapa local. Por isso, a etapa de nuvem pode ser iniciada mesmo que uma cópia individual tenha falhado; “backup concluído” não garante que todos os arquivos foram copiados.
- `CloudBackupFolders` deve corresponder a pastas na raiz de `D:`. A etapa de nuvem não copia diretamente toda a árvore `D:\Backups`, a menos que `Backups` esteja na lista.
- O projeto é compilado como `WinExe`. Embora o código use `Console.WriteLine`, uma execução normal do `.exe` pode não apresentar essas mensagens em um console; consulte o arquivo de log.
