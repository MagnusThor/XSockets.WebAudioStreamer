using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XSockets.Core.XSocket;
using XSockets.Core.XSocket.Helpers;
using XSockets.Core.XSocket.Model;

namespace AudioStreamer.Modules
{
    /// <summary>
    /// Very basic controller that serves a few mp3's
    /// Each client can ask for a file and it will be streamed in chunks back and instantly played at the JavaScript client. 
    /// </summary>
    public class AudioStream : XSocketController
    {
        const int ChunkSize = 500 * 1024;
        public static string AudioFilePath { get; set; }
        public static List<string> AudioFiles { get; set; }
        private IList<byte> FileBytes { get; set; }    
        public int BytesRead { get; set; }

        static AudioStream()
        {
            //get mp3's from the servers bin folder.
            AudioFilePath = XSockets.Plugin.Framework.Helpers.PluginHelpers.GetAssemblyDirectory();
            AudioFiles = Directory.EnumerateFiles(AudioFilePath, "*.mp3").ToList();
        }
        
        /// <summary>
        /// Send back avaialble tracks to the client asking for them
        /// </summary>
        /// <returns></returns>
        public async Task GetSongs()
        {
            var songs = AudioFiles.Select(audioFile => Regex.Replace(audioFile, @"(^[\w]:\\)([\w].+\w\\)", string.Empty)).ToList();
            await this.Invoke(songs, "songs");
        }

        /// <summary>
        /// Get a specific song by name.
        /// This will send back some metadata about the song
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task GetSong(string name)
        {
            this.BytesRead = 0;
            this.FileBytes = File.ReadAllBytes(AudioFilePath + "\\" + name);
            await this.Invoke(new { loaded = true, size = FileBytes.Count() }, "songloaded");
        }

        /// <summary>
        /// The client will ask for chunks within a specific time-range to keep data in the buffer client-side
        /// </summary>
        /// <returns></returns>
        public async Task GetChunk()
        {
            var arrayBuffer = FileBytes.Skip(this.BytesRead).Take(ChunkSize).ToArray();
            this.BytesRead = this.BytesRead + ChunkSize;

            var bm = new Message(arrayBuffer, new
            {
                size = FileBytes.Count(),
                read = this.BytesRead,
                final = this.BytesRead >= this.FileBytes.Count
            }, "chunk", this.Alias);
            await this.Invoke(bm);
        }

    }
}
