using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Collections;
using System.Threading;

namespace BeHappy
{

	public enum AviSynthColorspace:int
	{
		Unknown = 0,
		YV12    = -1610612728,
		RGB24   = +1342177281,
		RGB32   = +1342177282,
		YUY2    = -1610612740,
		I420    = -1610612720,
		IYUV    = I420
	}

    [SerializableAttribute]
	public class AviSynthException:ApplicationException
	{
#if NET8_0_OR_GREATER
        [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
        public AviSynthException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public AviSynthException(string message) : base(message)
		{
		}

		public AviSynthException(): base()
		{
		}

		public AviSynthException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}


	public enum AudioSampleType:int
	{
		Unknown=0,
		INT8  = 1,
		INT16 = 2, 
		INT24 = 4,    // Int24 is a very stupid thing to code, but it's supported by some hardware.
		INT32 = 8,
		FLOAT = 16
	};

	public sealed class AviSynthScriptEnvironment: IDisposable
	{
		public static string GetLastError()
		{
			return null;	
		}

		public AviSynthScriptEnvironment()
		{
		}

		public IntPtr Handle
		{
			get
			{
				return new IntPtr(0);
			}
		}

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static AviSynthClip OpenScriptFile(string filePath)
        {
            return new AviSynthClip("Import", filePath, false, true);
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static AviSynthClip OpenScriptFile(string filePath, bool bRequireRGB24)
        {
            return new AviSynthClip("Import", filePath, bRequireRGB24, true);
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static AviSynthClip ParseScript(string script)
        {
            return new AviSynthClip("Eval", script, false, true);
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static AviSynthClip ParseScript(string script, bool bRequireRGB24)
        {
            return new AviSynthClip("Eval", script, bRequireRGB24, true);
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static AviSynthClip ParseScript(string script, bool bRequireRGB24, bool runInThread)
        {
            return new AviSynthClip("Eval", script, bRequireRGB24, runInThread);
        }


        public void Dispose()
		{
			
		}
	}

	/// <summary>
	/// Summary description for AviSynthClip.
	/// </summary>
	public class AviSynthClip: IDisposable
	{
		#region PInvoke related stuff
		[StructLayout(LayoutKind.Sequential)]
			struct AVSDLLVideoInfo
		{
			public int width;
			public int height;
			public int raten;
			public int rated;
			public int aspectn;
			public int aspectd;
			public int interlaced_frame;
			public int top_field_first;
			public int num_frames;
			public AviSynthColorspace pixel_type;

			// Audio
			public int audio_samples_per_second;
			public AudioSampleType sample_type;
			public int nchannels;
			public int num_audio_frames;
			public long num_audio_samples;
            public int channelMask;
        }


        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_init_2(ref IntPtr avs, string func, string arg, ref AVSDLLVideoInfo vi, ref AviSynthColorspace originalColorspace, ref AudioSampleType originalSampleType, string cs);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_init_3(ref IntPtr avs, string func, string arg, ref AVSDLLVideoInfo vi, ref AviSynthColorspace originalColorspace, ref AudioSampleType originalSampleType, string cs);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_destroy(ref IntPtr avs);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_getlasterror(IntPtr avs, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sb, int len);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_getaframe(IntPtr avs, IntPtr buf, long sampleNo, long sampleCount);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_getvframe(IntPtr avs, IntPtr buf, int stride, int frm);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_getintvariable(IntPtr avs, string name, ref int val);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_getinterfaceversion(ref int val);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_getstrfunction(IntPtr avs, string func, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sb, int len);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_functionexists(IntPtr avs, string func, ref bool val);
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool LoadLibraryA(string hModule);
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool GetModuleHandleExA(int dwFlags, string ModuleName, IntPtr phModule);
        [DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        #endregion

        private IntPtr _avs;
        private AVSDLLVideoInfo _vi;
        private AviSynthColorspace _colorSpace;
        private AudioSampleType _sampleType;
        private static object _locker = new object();
        private static object _lockerAccessCounter = new object();
        private static object _lockerDLL = new object();
        private int _countAccess = 0;
        private int _random;

        private string GetLastError()
        {
            const int errlen = 1024;
            StringBuilder sb = new StringBuilder(errlen);
            try
            {
                if (_avs != IntPtr.Zero)
                {
                    AccessCounter(true);
                    sb.Length = dimzon_avs_getlasterror(_avs, sb, errlen);
                    AccessCounter(false);
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(ex.Message);
            }
            return sb.ToString();
        }

        #region Clip Properties

        public bool HasVideo
		{
			get
			{
				return VideoWidth > 0 && VideoHeight > 0;
			}
		}

		public int VideoWidth
		{
			get
			{
				return _vi.width;
			}
		}
		public int VideoHeight
		{
			get
			{
				return _vi.height;
			}
		}
		public int raten
		{
			get
			{
				return _vi.raten;
			}
		}
		public int rated
		{
			get
			{
				return _vi.rated;
			}
		}
		public int aspectn
		{
			get
			{
				return _vi.aspectn;
			}
		}
		public int aspectd
		{
			get
			{
				return _vi.aspectd;
			}
		}
		public int interlaced_frame
		{
			get
			{
				return _vi.interlaced_frame;
			}
		}
		public int top_field_first
		{
			get
			{
				return _vi.top_field_first;
			}
		}
		public int num_frames
		{
			get
			{
				return _vi.num_frames;
			}
		}
        // Audio
		public bool HasAudio
        {
            get
            {
                return _vi.num_audio_samples > 0;
            }
        }

        public int AudioSampleRate
		{
			get
			{
				return _vi.audio_samples_per_second;
			}
		}

		public long SamplesCount
		{
			get
			{
				return _vi.num_audio_samples;
			}
		}
		public AudioSampleType SampleType
		{
			get
			{
				return _vi.sample_type;
			}
		}
		public short ChannelsCount
		{
			get
			{
				return (short)_vi.nchannels;
			}
		}
        public int ChannelMask
        {
            get
            {
                return _vi.channelMask;
            }

        }

        public AviSynthColorspace PixelType
		{
			get
			{
				return _vi.pixel_type;
			}
		}

		public AviSynthColorspace OriginalColorspace
		{
			get
			{
				return _colorSpace;
			}
		}
		public AudioSampleType OriginalSampleType
		{
			get
			{
				return _sampleType;
			}
		}
        public string GetStrFunction(string strFunction)
        {
            const int errlen = 1024;
            StringBuilder sb = new StringBuilder(errlen);
            if (_avs != IntPtr.Zero)
            {
                AccessCounter(true);
                sb.Length = dimzon_avs_getstrfunction(_avs, strFunction, sb, errlen);
                AccessCounter(false);
            }
            return sb.ToString();
        }
        public int GetIntVariable(string variableName, int defaultValue)
        {
            int v = 0;
            int res = 0;
            if (_avs != IntPtr.Zero)
            {
                AccessCounter(true);
                res = dimzon_avs_getintvariable(this._avs, variableName, ref v);
                AccessCounter(false);
            }
            if (res < 0)
                throw new AviSynthException(GetLastError());
            return (0 == res) ? v : defaultValue;
        }


        #endregion

        public void ReadAudio(IntPtr addr, long offset, int count)
		{
            if (_avs != IntPtr.Zero)
            {
                AccessCounter(true);
                if (0 != dimzon_avs_getaframe(_avs, addr, offset, count))
                {
                    AccessCounter(false);
                    throw new AviSynthException(GetLastError());
                }
                AccessCounter(false);
            }
        }

		public void ReadAudio(byte buffer, long offset, int count)
		{
			GCHandle h = GCHandle.Alloc(buffer,GCHandleType.Pinned);
			try
			{
				ReadAudio(h.AddrOfPinnedObject(), offset, count);
			}
			finally
			{
				h.Free();
			}
		}

		public void ReadFrame(IntPtr addr, int stride, int frame)
		{
            if (_avs != IntPtr.Zero)
            {
                AccessCounter(true);
                if (0 != dimzon_avs_getvframe(_avs, addr, stride, frame))
                {
                    AccessCounter(false);
                    throw new AviSynthException(GetLastError());
                }
                AccessCounter(false);
            }
        }

        /// <summary>
        /// Gets the AviSynth interface version of the AviSynthWrapper.dll
        /// </summary>
        /// <returns></returns>
        public static int GetAvisynthWrapperInterfaceVersion()
        {
            int iVersion = 0;
            try
            {
                int iResult = dimzon_avs_getinterfaceversion(ref iVersion);
            }
            catch (Exception) { }
            return iVersion;
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public AviSynthClip(string func, string arg, bool bRequireRGB24, bool bRunInThread)
        {
            _vi = new AVSDLLVideoInfo();
            _avs = IntPtr.Zero;
            _colorSpace = AviSynthColorspace.Unknown;
            _sampleType = AudioSampleType.Unknown;
            bool bOpenSuccess = false;
            string strErrorMessage = string.Empty;

            lock (_locker)
            {
                Random rnd = new Random();
                _random = rnd.Next(1, 1000000);

                System.Windows.Forms.Application.UseWaitCursor = true;
                if (bRunInThread)
                {
                    Thread t = new Thread(new ThreadStart(delegate
                    {
                        bOpenSuccess = OpenAVSScript(func, arg, bRequireRGB24, out strErrorMessage);
                    }));
                    t.Start();
                    while (t.ThreadState == ThreadState.Running)
                        Thread.Sleep(1000);
                }
                else
                {
                    bOpenSuccess = OpenAVSScript(func, arg, bRequireRGB24, out strErrorMessage);
                }
                System.Windows.Forms.Application.UseWaitCursor = false;
            }

            if (bOpenSuccess == false)
            {
                string err = string.Empty;
                if (_avs != IntPtr.Zero)
                    err = GetLastError();
                else
                    err = strErrorMessage;
                Dispose(false);
                throw new AviSynthException(err);
            }
        }


        private void cleanup(bool disposing)
		{
			dimzon_avs_destroy(ref _avs);
			_avs = new IntPtr(0);
			if(disposing)
				GC.SuppressFinalize(this);
		}

		~AviSynthClip()
		{
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_avs == IntPtr.Zero)
                return;

            // wait till the avs object is not used anymore
            while (_countAccess > 0)
                Thread.Sleep(100);
            _ = dimzon_avs_destroy(ref _avs);
            if (_avs != IntPtr.Zero)
                CloseHandle(_avs);
            _avs = IntPtr.Zero;
            if (disposing)
                GC.SuppressFinalize(this);
                //HandleAviSynthWrapperDLL(true, String.Empty);
        }

        private bool OpenAVSScript(string func, string arg, bool bRequireRGB24, out string strErrorMessage)
        {
            bool bOpenSuccess = false;
            strErrorMessage = string.Empty;
            try
            {
                if (0 == dimzon_avs_init_3(ref _avs, func, arg, ref _vi, ref _colorSpace, ref _sampleType,
                        bRequireRGB24 ? AviSynthColorspace.RGB24.ToString() : AviSynthColorspace.Unknown.ToString()))
                        bOpenSuccess = true;

                if (!bOpenSuccess)
                {
                    // fallback to the old function
                    if (0 == dimzon_avs_init_2(ref _avs, func, arg, ref _vi, ref _colorSpace, ref _sampleType, AviSynthColorspace.RGB24.ToString()))
                        bOpenSuccess = true;
                }
            }
            catch (Exception ex)
            {
                strErrorMessage = ex.Message;
            }
            return bOpenSuccess;
        }

        private void AccessCounter(bool bAdd)
        {
            lock (_lockerAccessCounter)
            {
                if (bAdd)
                    _countAccess++;
                else
                    _countAccess--;
            }
        }
        public short BitsPerSample
		{
			get
			{
				return (short)(BytesPerSample*8);
			}
		}
		public short BytesPerSample
		{
			get
			{
				switch (SampleType) 
				{
					case AudioSampleType.INT8:
						return 1;
					case AudioSampleType.INT16:
						return 2;
					case AudioSampleType.INT24:
						return 3;
					case AudioSampleType.INT32:
						return 4;
					case AudioSampleType.FLOAT:
						return 4;
					default:
						throw new ArgumentException(SampleType.ToString());
				}
			}
		}

		public int AvgBytesPerSec
		{
			get
			{
				return AudioSampleRate * ChannelsCount * BytesPerSample;
			}
		}

		public long AudioSizeInBytes
		{
			get
			{
				return SamplesCount * ChannelsCount * BytesPerSample;
			}
		}

	}
}
