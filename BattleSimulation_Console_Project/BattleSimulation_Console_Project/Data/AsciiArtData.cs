public static class AsciiArtData
{
    // 구조체로 래핑
    // 축자 문자열(가독성 향상)
    public static readonly AsciiArt Title = new(
    [
        @" ____        _   _   _                             ",
        @"| __ )  __ _| |_| |_| | ___                        ",
        @"|  _ \ / _` | __| __| |/ _ \                       ",
        @"| |_) | (_| | |_| |_| |  __/                       ",
        @"|____/ \__,_|\__|\__|_|\___|     _   _             ",
        @"/ ___|(_)_ __ ___  _   _| | __ _| |_(_) ___  _ __  ",
        @"\___ \| | '_ ` _ \| | | | |/ _` | __| |/ _ \| '_ \ ",
        @" ___) | | | | | | | |_| | | (_| | |_| | (_) | | | |",
        @"|____/|_|_| |_| |_|\__,_|_|\__,_|\__|_|\___/|_| |_|"
    ]);

    public static readonly AsciiArt Victory = new(
    [
        @"__     _____ ____ _____ ___  ______   __",
        @"\ \   / /_ _/ ___|_   _/ _ \|  _ \ \ / /",
        @" \ \ / / | | |     | || | | | |_) \ V / ",
        @"  \ V /  | | |___  | || |_| |  _ < | |  ",
        @"   \_/  |___\____| |_| \___/|_| \_\|_|  "
    ]);

    public static readonly AsciiArt GameOver = new(
        [
        @"   ____                               ",
        @"  / ___| __ _ _ __ ___   ___          ",
        @" | |  _ / _` | '_ ` _ \ / _ \         ",
        @" | |_| | (_| | | | | | |  __/         ",
        @"  \____|\__,_|_| |_| |_|\___|         ",
        @"    / _ \__   _____ _ __              ",
        @"   | | | \ \ / / _ \ '__|             ",
        @"   | |_| |\ V /  __/ |     _   _   _  ", 
        @"    \___/  \_/ \___|_|    (_) (_) (_) "
        ]);

    public static readonly AsciiArt Ghost = new(
    [
        @"  .---.  ",
        @" /     \ ",
        @"| (O) (O)|",
        @"|   V   |",
        @"|  _|_  |",
        @" '-----' "
    ]);
}

public struct AsciiArt
{
    public string[] Lines { get; }
    public int Width { get; }
    public int Height { get; }

    public AsciiArt(string[] lines)
    {
        Lines = lines;
        Height = lines.Length;
        
        // 가장 긴 줄을 찾아 Width로 설정
        int maxWidth = 0;
        foreach (var line in lines)
        {
            if (line.Length > maxWidth) maxWidth = line.Length;
        }
        Width = maxWidth;
    }
}