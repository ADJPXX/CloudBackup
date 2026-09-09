namespace CloudBackup.Services;

public static class LogService
{
    public static void StartExecution(string type)
    {
        var now = DateTime.Now;

        var header = 
            "==================================================\n" +
            $"{type}: {now:dd/MM/yyyy HH:mm:ss}\n" + 
            "==================================================\n";
        
        File.AppendAllText(PathsService.LogPath, header);
    }


    public static void AddLog(string log)
    {
        var time = DateTime.Now;
        
        File.AppendAllText(PathsService.LogPath, $"[ {time:HH:mm:ss} ] {log}\n");
    }
    
    
    public static void EndExecution()
    {
        File.AppendAllText(PathsService.LogPath, "\n");
    }
}