using System.Diagnostics;

namespace CloudBackup.Services;

public static class RobocopyService
{
    public static async Task<(string, string)> CopyAsync(string arguments)
    {
        try
        {
            var backup = Process.Start(new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });

            var error = await backup?.StandardError.ReadToEndAsync()!;

            await backup.WaitForExitAsync();

            var status = RobocopyInterpreter(backup.ExitCode);

            return (status, error);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NA FUNÇÃO \"CopyAsync()\": {ex.Message}");
            return (string.Empty, string.Empty);
        }
    }


    private static string RobocopyInterpreter(int exitCode)
    {
        return exitCode switch
        {
            0 => "Nada para copiar",
            1 => "Copiado",
            2 => "Existem extras",
            3 => "Copiado + existem extras",
            4 => "Existem arquivos incompatíveis",
            5 => "Copiado + incompatíveis",
            6 => "Extras + incompatíveis",
            7 => "Copiado + extras + incompatíveis",
            8 => "Falha de cópia",
            9 => "Copiado algo + houve falha",
            10 => "Extras + falha",
            11 => "Copiado + extras + falha",
            12 => "Incompatíveis + falha",
            13 => "Copiado + incompatíveis + falha",
            14 => "Extras + incompatíveis + falha",
            15 => "Copiado + extras + incompatíveis + falha",
            16 => "Erro fatal",
            _ => "Código de saída desconhecido"
        };
    }
}