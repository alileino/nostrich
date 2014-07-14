using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using System.IO;
using NAudio.Wave.SampleProviders;
namespace WesternLib
{
    public class GameMusic
    {
        private LoopStream _loopingStream;
        public string Name { get; set; }
        public WaveStream Stream { get { return _loopingStream; } }

        public GameMusic(string name, string filename, float volume =0.075f)
        {
            WaveChannel32 stream = GameSound.CreateInputStream(filename);
            stream.Volume = volume;
            Name = name;
            _loopingStream = new LoopStream(stream);
            
        }
    }

    public class GameSound
    {
        WaveChannel32 _waveStream;
        public string Name { get; set; }
        public WaveStream Stream { get { return _waveStream; } }
        public bool AutoRestart { get; set; }
        public bool Ended { get { return _waveStream.Position >= _waveStream.Length; } }

        public GameSound(string filename, bool autoRestart=true, float volume=1f)
        {
            AutoRestart = autoRestart;
            _waveStream = CreateInputStream(filename);
            _waveStream.Volume = volume;
            Stop();
        }

        public void Play()
        {
            if(AutoRestart || Ended)
                _waveStream.Position = 0;
        }

        public void Stop()
        {
           // _waveStream.Skip((int)(_waveStream.TotalTime.TotalSeconds + 0.5f)-10);
            _waveStream.Seek(0, SeekOrigin.End);

        }

        public static SampleChannel CreateInputSample(string filename)
        {

            //WaveStream mp3Reader = new Mp3FileReader(filename);

            WaveStream mp3Reader = new Mp3FileReader(filename);
            
            var ss = new SampleChannel(mp3Reader);
            ss.Volume = 0.2f;
            return ss;
        }

        public static WaveChannel32 CreateInputStream(string filename, float volume = 1.0f)
        {

            //AudioFileReader fileReader = new AudioFileReader(filename);
            //var fileSampleProvider = fileReader.ToSampleProvider();
            //var fileSampleChannel = new SampleChannel(fileReader);
            //fileReader.Volume = volume;
            WaveChannel32 inputStream;
            if (filename.EndsWith(".mp3"))
            {
                //WaveStream mp3Reader = new Mp3FileReader(filename);
                
                WaveStream mp3Reader = new Mp3FileReader(filename);
                var ss = new SampleChannel(mp3Reader);
                ss.Volume = 0.1f;
                inputStream = new WaveChannel32(mp3Reader);
                
            }
            else if (filename.EndsWith(".wav"))
            {
                WaveStream waveReader = new WaveFileReader(filename);
                inputStream = new WaveChannel32(waveReader);
            }
            else
            {
                throw new InvalidOperationException("Unsupported extension");
            }
            return inputStream;
        }
    }
    public class SoundManager
    {
        WaveMixerStream32 _soundMixer;
        WaveMixerStream32 _musicMixer;
        WaveOutEvent waveOutDevice;
        WaveStream mainOutputStream;
        WaveOutEvent waveOutDeviceMusic;

        private Dictionary<string, GameSound> _sounds = new Dictionary<string,GameSound>();
        private Dictionary<string, GameMusic> _music = new Dictionary<string, GameMusic>();
        private GameMusic _currentSong = null;

        public SoundManager()
        {
            
            _soundMixer = new WaveMixerStream32();
            _soundMixer.AutoStop = false;
            _musicMixer = new WaveMixerStream32();
            _musicMixer.AutoStop = false;
            waveOutDevice = new WaveOutEvent();
            waveOutDeviceMusic = new WaveOutEvent();
            
            waveOutDevice.Init(_soundMixer);
            waveOutDeviceMusic.Init(_musicMixer);
            waveOutDeviceMusic.Play();
            waveOutDevice.Play();
            ScriptBindings.ScriptPlaySound += this.PlaySound;
        }

        public void Update()
        { 

        }

        public void LoadSound(string name, string filename, bool autoRestart=true, float volume=1f)
        {
            filename = Path.Combine("Sound", filename);
            GameSound sound = new GameSound(filename, autoRestart,volume);
            _sounds.Add(name, sound);
            _soundMixer.AddInputStream(sound.Stream);
            sound.Stop();
        }


        public void LoadMusic(string name, string filename)
        {
            filename = Path.Combine("Sound", filename);
            GameMusic music = new GameMusic(name,filename);
            _music.Add(name, music);
        }

        public void PlayMusic(string name)
        {
            if (_currentSong != null)
            {
                if (_currentSong.Name == name)
                    return;

                _musicMixer.RemoveInputStream(_currentSong.Stream);
                
            }
            GameMusic music = _music[name];
            _musicMixer.AddInputStream(music.Stream);
            _currentSong = music;
        }

        public void PlaySound(string name)
        {
            //GameSound sound = _sounds[name];
            _sounds[name].Play();
        }

        void waveOutDevice_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Console.WriteLine("Playback stopped");
        }

        private WaveStream CreateInputStream(string fileName)
        {
            WaveChannel32 inputStream;
            if (fileName.EndsWith(".mp3"))
            {
                WaveStream mp3Reader = new Mp3FileReader(fileName);
                inputStream = new WaveChannel32(mp3Reader);

            }
            else if (fileName.EndsWith(".wav"))
            {
                WaveStream waveReader = new WaveFileReader(fileName);
                inputStream = new WaveChannel32(waveReader);
            }
            else
            {
                throw new InvalidOperationException("Unsupported extension");
            }
            return inputStream;
        }

        private void CloseWaveOut()
        {
            if (waveOutDevice != null)
            {
                waveOutDevice.Stop();
            }
            if (mainOutputStream != null)
            {
                // this one does the metering stream
                mainOutputStream.Close();
                mainOutputStream = null;
            }
            if (waveOutDevice != null)
            {
                waveOutDevice.Dispose();
                waveOutDevice = null;
            }
        }

    }

    class LoopStream : WaveStream
    {
        WaveChannel32 sourceStream;

        public LoopStream(WaveChannel32 source)
        {
            this.sourceStream = source;
            
        }

        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        public override long Length
        {
            get { return long.MaxValue / 32; }
        }

        public override long Position
        {
            get
            {
                return sourceStream.Position;
            }
            set
            {
                sourceStream.Position = value;
            }
        }

        public override bool HasData(int count)
        {
            // infinite loop
            return true;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = 0;
            while (read < count)
            {
                int required = count - read;
                int readThisTime = sourceStream.Read(buffer, offset + read, required);
                if (readThisTime < required)
                {
                    sourceStream.Position = 0;
                }

                if (sourceStream.Position >= sourceStream.Length)
                {
                    sourceStream.Position = 0;
                }
                read += readThisTime;
            }
            return read;
        }

        protected override void Dispose(bool disposing)
        {
            sourceStream.Dispose();
            base.Dispose(disposing);
        }
    }

}
