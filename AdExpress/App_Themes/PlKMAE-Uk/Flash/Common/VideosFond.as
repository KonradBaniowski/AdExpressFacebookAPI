package 
{
    import fl.video.*;
    import flash.display.*;
    import flash.events.*;

    public class VideosFond extends MovieClip
    {
        public var bordDegradeBas:MovieClip;
        private var iCurIndexVideo:int;
        private var iCurVideo:int = -1;
        private var aVideosWidth:Array;
        public var bordDegradeDroite:MovieClip;
        public var fondu:MovieClip;
        private var aVideos:Array;
        public var video:FLVPlayback;

        public function VideosFond()
        {
            this.aVideos = new Array();
            this.aVideosWidth = new Array();
            addEventListener(Event.ADDED_TO_STAGE, this.initOnLoad);
            this.aVideos.push("videos/360_HQ.flv");
            this.aVideosWidth.push(980);          
            return;
        }// end function

        private function setNextVideoInQueue() : void
        {
            if ((this.iCurVideo + 1) >= this.aVideos.length)
            {
                this.iCurVideo = 0;
            }
            else
            {
                var _loc_1:String = this;
                var _loc_2:* = this.iCurVideo + 1;
                //_loc_1.iCurVideo = _loc_2;
            }
            this.video.source = this.aVideos[this.iCurIndexVideo];
            this.video.width = this.aVideosWidth[this.iCurIndexVideo];
            this.video.height = this.video.width * 560 / 980;
            if (this.video.width < 936)
            {
                this.bordDegradeBas.y = this.video.y + this.video.height - this.bordDegradeBas.height;
                this.bordDegradeDroite.x = this.video.x + this.video.width - this.bordDegradeDroite.width;
            }
            this.video.play();
            return;
        }// end function

        function __setProp_video_Scene1_Video_0()
        {
            try
            {
                this.video["componentInspectorSetting"] = true;
            }
            catch (e:Error)
            {
            }
            this.video.align = "center";
            this.video.autoPlay = true;
            this.video.isLive = false;
            this.video.scaleMode = "maintainAspectRatio";
            this.video.skin = "";
            this.video.skinAutoHide = false;
            this.video.skinBackgroundAlpha = 0.85;
            this.video.skinBackgroundColor = 4697035;
            this.video.source = "";
            this.video.volume = 1;
            try
            {
                this.video["componentInspectorSetting"] = false;
            }
            catch (e:Error)
            {
            }
            return;
        }// end function

        private function restartTheVideo(event:Event) : void
        {
            this.video.playheadUpdateInterval = 0;
            this.video.play();
            return;
        }// end function

        private function hideFondu(event:Event) : void
        {
            return;
        }// end function

        private function initOnLoad(event:Event) : void
        {
            this.video.addEventListener("complete", this.restartTheVideo, false, 0, true);
            this.fondu.alpha = 0;
            this.setNextVideoInQueue();
            return;
        }// end function

        private function randomizeArray(param1:Array) : Array
        {
            var _loc_2:* = new Array();
            while (param1.length > 0)
            {
                
                _loc_2.push(param1.splice(Math.floor(Math.random() * param1.length), 1));
            }
            return _loc_2;
        }// end function

        private function showFondu(event:Event) : void
        {
            return;
        }// end function

    }
}
