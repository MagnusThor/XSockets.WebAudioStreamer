

var MediaSourceChunksPlayer = (function () {
    var ctor = function (audioEl) {
        var self = this;
        var mediaSource = new MediaSource();
        this.audioEl = document.querySelector(audioEl);
        var sourceBuffer;
        var loadedBuffers = [];
        var itemsAppendedToSourceBuffer = 0;
        this.addChunk = function(result) {
            loadedBuffers.push(result);

            if (!sourceBuffer.updating) {
                getNextBuffer();
            }
            if (loadedBuffers.length == 0) {
                startPlayback();
            }
        }

        function getNextBuffer() {
            if (loadedBuffers.length) {
                sourceBuffer.appendBuffer(loadedBuffers.shift());
                itemsAppendedToSourceBuffer++;
            }
        }
        function sourceOpenCallback() {
            sourceBuffer = mediaSource.addSourceBuffer('audio/mpeg');
            sourceBuffer.addEventListener('updateend', getNextBuffer, false);
            console.log("sourceOpenCallback");
        }

        function sourceCloseCallback() {
            mediaSource.removeSourceBuffer(sourceBuffer);
        }

        function sourceEndedCallback() {
        }
        function startPlayback() {
            if (self.audioEl.paused) {
                self.audioEl.play();
            }
        }

        mediaSource.addEventListener('sourceopen', sourceOpenCallback, false);
        mediaSource.addEventListener('webkitsourceopen', sourceOpenCallback, false);
        mediaSource.addEventListener('sourceclose', sourceCloseCallback, false);
        mediaSource.addEventListener('webkitsourceclose', sourceCloseCallback, false);
        mediaSource.addEventListener('sourceended', sourceEndedCallback, false);
        mediaSource.addEventListener('webkitsourceended', sourceEndedCallback, false);

        self.audioEl.src = window.URL.createObjectURL(mediaSource);
 
        var audioContext = new AudioContext();
        var source = audioContext.createMediaElementSource(self.audioEl);
        source.connect(audioContext.destination);

    }


    return ctor;


}())