﻿namespace RunTrack
{
    public class RundenArt
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int LaengeInMeter { get; set; } = 1000;
        public int MaxScanIntervalInSekunden { get; set; } = 60;

        public override string ToString()
        {
            return Name;
        }
    }
}
