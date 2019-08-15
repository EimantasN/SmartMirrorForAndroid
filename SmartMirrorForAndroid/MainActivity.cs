using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using System;
using System.Threading.Tasks;
using Library;
using Square.Picasso;
using Android.Content;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using System.Linq;
using Android.Speech.Tts;
using Android.Runtime;
using Java.Util;
using Android.Speech;
using Android.Media;
using Xamarin.Cognitive.Face.Droid;
using SmartMirrorForAndroid.Recognition;

namespace SmartMirrorForAndroid
{
    [Activity(Label = "@string/app_name", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IRecognitionListener, TextToSpeech.IOnInitListener
    {
        //Capera picture
        ImageView camereImg;


        //Face recognition
        FaceRecognition MyServiceClinet = new FaceRecognition();

        Button recognition;
        TextView recognitionText;

        //SPEECH
        private const int VOICE = 10;

        int musicOrigVol;
        AudioManager audioManager;

        private SpeechRecognizer sr;
        TextToSpeech tts;
        Button textToSpeech;
        TextView SpeechText;

        Button SpeechToText;
        TextView lisenTome;

        DataParsing parsing;
        TemperatureNow temperatureData;
        TextClock clock;
        TextView console;
        TextView Temperature;

        TextView Wind;
        TextView Pressure;
        TextView Humidity;
        TextView Water;

        TextView Battery;
        TextView brightness;

        ImageView TempImage;

        //Recycle vie Setup

        private static MyList<LinkomanijosData> List;

        private RecyclerView.LayoutManager mLayoutManager;
        private static RecyclerView.Adapter mAdapter;
        private static RecyclerView mRecyclerView;

        //First To city recycleView
        private static MyList<TrafiListModel> ToCityList;

        private RecyclerView.LayoutManager mLayoutManagerToCity;
        private static RecyclerView.Adapter mAdapterToCity;
        private static RecyclerView mRecyclerViewToCity;

        //First To Gym recycleView
        private static MyList<TrafiListModel> ToGymList;

        private RecyclerView.LayoutManager mLayoutManagerToGym;
        private static RecyclerView.Adapter mAdapterToGym;
        private static RecyclerView mRecyclerViewToGym;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Fullscreen
            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_main);

            console = FindViewById<TextView>(Resource.Id.console);
            string a = DateTime.Now.ToLocalTime().ToString();
            clock = FindViewById<TextClock>(Resource.Id.textView2);
            Temperature = FindViewById<TextView>(Resource.Id.textView3);
            TempImage = FindViewById<ImageView>(Resource.Id.imageView2);

            //Temperature data
            Wind = FindViewById<TextView>(Resource.Id.windText);
            Pressure = FindViewById<TextView>(Resource.Id.pressureText);
            Humidity = FindViewById<TextView>(Resource.Id.humidityText);
            Water = FindViewById<TextView>(Resource.Id.waterText);

            Battery = FindViewById<TextView>(Resource.Id.batterText);
            brightness = FindViewById<TextView>(Resource.Id.brightnessText);

            SetBrightness(100);

            parsing = new DataParsing();
            //Temperature task updater (10 sec)
            Task.Run(() =>
            {
                updateWithInterval();
                BatteryManagement();
            });

            ToCityList = new MyList<TrafiListModel>();
            mRecyclerViewToCity = FindViewById<RecyclerView>(Resource.Id.recyclerviewToCity);
            mLayoutManagerToCity = new LinearLayoutManager(this);
            mRecyclerViewToCity.SetLayoutManager(mLayoutManagerToCity);
            mAdapterToCity = new RecyclerAdapterTrafi(ToCityList, mRecyclerViewToCity, this);
            ToCityList.Adapter = mAdapterToCity;
            mRecyclerViewToCity.SetAdapter(mAdapterToCity);


            ToGymList = new MyList<TrafiListModel>();
            mRecyclerViewToGym = FindViewById<RecyclerView>(Resource.Id.recyclerviewToGym);
            mLayoutManagerToGym = new LinearLayoutManager(this);
            mRecyclerViewToGym.SetLayoutManager(mLayoutManagerToGym);
            mAdapterToGym = new RecyclerAdapterTrafi(ToGymList, mRecyclerViewToGym, this);
            ToGymList.Adapter = mAdapterToGym;
            mRecyclerViewToGym.SetAdapter(mAdapterToGym);

            List = new MyList<LinkomanijosData>();
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerview);
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            mAdapter = new RecyclerAdapter(List, mRecyclerView);
            List.Adapter = mAdapter;
            mRecyclerView.SetAdapter(mAdapter);

            //SPEECH

            //For audio control
            audioManager = (AudioManager)GetSystemService(Context.AudioService);

            textToSpeech = FindViewById<Button>(Resource.Id.button);
            SpeechText = FindViewById<TextView>(Resource.Id.textView27);

            SpeechToText = FindViewById<Button>(Resource.Id.button2);
            lisenTome = FindViewById<TextView>(Resource.Id.textView28);

            tts = new TextToSpeech(this, this);

            sr = SpeechRecognizer.CreateSpeechRecognizer(this);
            sr.SetRecognitionListener(this);

            textToSpeech.Click += TextToSpeech_Click;

            SpeechToText.Click += (sender, e) =>
            {
                musicOrigVol = audioManager.GetStreamVolume(Stream.Music);
                audioManager.SetStreamVolume(Stream.Music, 0, 0);
                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraCallingPackage, "this package");
                intent.PutExtra(RecognizerIntent.ExtraMaxResults, 5);
                sr.StartListening(intent);
            };
            //SpeechToText.Click += RecordVoice;

            recognition = FindViewById<Button>(Resource.Id.button3);
            recognitionText = FindViewById<TextView>(Resource.Id.textView6);


            camereImg = FindViewById<ImageView>(Resource.Id.imageView);
            recognition.Click += async delegate
            {
                //static final int REQUEST_IMAGE_CAPTURE = 1;
                //Intent takePictureIntent = new Intent(MediaStore.ActionImageCapture);
                //if (takePictureIntent.ResolveActivity(PackageManager) != null)
                //{
                //    StartActivityForResult(takePictureIntent, 1);
                //}


                var classs = new FaceRecognition();
                string path = @"/data/user/0/camerapictureTaking.camerapictureTaking/filesEimantas.jpg";
                recognitionText.Text = await classs.RecognitionFace("1", path);
                SpeechText.Text = "Authorization succeeded, Hello came back " + recognitionText.Text;
                //Speak();
            };

        }

        //protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        //{
        //    base.OnActivityResult(requestCode, resultCode, data);
        //    Bitmap bitmap = (Bitmap)data.Extras.Get("data");
        //    camereImg.SetImageBitmap(bitmap);
        //}

        private void TextToSpeech_Click(object sender, EventArgs e)
        {
            Speak();
        }

        public void WriteToConsole(string newtext, TextView console)
        {
            Application.SynchronizationContext.Post(_ => { console.Text = newtext + "  time - " + DateTime.Now.ToLocalTime().ToString() + "\n" + console.Text; }, null);

        }

        public void BatteryManagement()
        {
            var filter = new IntentFilter(Intent.ActionBatteryChanged);
            var battery = RegisterReceiver(null, filter);
            int level = battery.GetIntExtra(BatteryManager.ExtraLevel, -1);
            int scale = battery.GetIntExtra(BatteryManager.ExtraScale, -1);

            int level_0_to_100 = (int)Math.Floor(level * 100D / scale);

            Application.SynchronizationContext.Post(_ => { Battery.Text = level_0_to_100.ToString() + " %"; }, null);
            Application.SynchronizationContext.Post(_ => { console.Text = string.Format("Battery Level: {0}", level_0_to_100) + "\n" + console.Text; }, null);
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        public async Task updateWithInterval()
        {
            ////temperatureData = parsing.ParseTemperatureData("Kaunas", console);
            //Application.SynchronizationContext.Post(_ => { Temperature.Text = temperatureData.Temperature; }, null);
            ////SetImage(temperatureData.image);

            //var time = parsing.TrafiApi();
            //Application.SynchronizationContext.Post(_ => { EraseCity(); }, null);
            //await Task.Delay(250);
            //time.HomeToAkro.Routes.First().RouteSegments.ForEach(x =>
            //{
            //    var model = new TrafiListModel
            //    {
            //        Image = x.IconUrl,
            //        StartTime = x.StartPoint.Time,
            //        EndTime = x.EndPoint.Time,
            //        EndStreet = x.EndPoint.Name,
            //        NextStopDistance = x.DistanceMeters + " m",
            //        NextStopTime = x.DurationMinutes + " min",
            //        ImageBottomDistance = x.WalkDistanceMeters.ToString()
            //    };
            //    Application.SynchronizationContext.Post(_ => { ToCityList.Add(model); }, null);

            //});
            //Application.SynchronizationContext.Post(_ => { EraseGym(); }, null);
            //await Task.Delay(250);
            //time.HomeToGym.Routes.First().RouteSegments.ForEach(x =>
            //{
            //    var model = new TrafiListModel
            //    {
            //        Image = x.IconUrl,
            //        StartTime = x.StartPoint.Time,
            //        EndTime = x.EndPoint.Time,
            //        EndStreet = x.EndPoint.Name,
            //        NextStopDistance = x.DistanceMeters + " m",
            //        NextStopTime = x.DurationMinutes + " min",
            //        ImageBottomDistance = x.WalkDistanceMeters.ToString()
            //    };
            //    Application.SynchronizationContext.Post(_ => { ToGymList.Add(model); }, null);

            //});
            //await Task.Delay(250);

            //Application.SynchronizationContext.Post(_ => { Erase(); }, null);


            //parsing.GetLinkomanijosHdMovies().ForEach(x =>
            //{
            //    Application.SynchronizationContext.Post(_ => { List.Add(x); }, null);
            //});

            //Application.SynchronizationContext.Post(_ => { Wind.Text = temperatureData.Wind; }, null);
            //Application.SynchronizationContext.Post(_ => { Pressure.Text = temperatureData.Pressure; }, null);
            //Application.SynchronizationContext.Post(_ => { Humidity.Text = temperatureData.Humidity; }, null);
            //Application.SynchronizationContext.Post(_ => { Water.Text = temperatureData.Water; }, null);
            //await Task.Delay(10000);
            //updateWithInterval();
        }

        public void SetImage(string x)
        {
            if (x.Contains("Fair"))
            {
                TempImage.SetImageResource(Resource.Drawable.sauleta);
            }
            else
            {
                TempImage.SetImageResource(Resource.Drawable.DebesuotaSaule);
            }
        }

        public void SetBrightness(float brightnessValue)
        {
            System.Random random = new System.Random();
            int a = random.Next(1, 100);
            var attributesWindow = new WindowManagerLayoutParams();

            attributesWindow.CopyFrom(Window.Attributes);

            attributesWindow.ScreenBrightness = (a / 100);
            brightness.Text = a + " %";

            Window.Attributes = attributesWindow;

        }

        #region Recycle View

        public class MyList<T>
        {
            private List<T> mItems;
            private RecyclerView.Adapter mAdapter;

            public void Erase()
            {
                mItems = new List<T>();
            }

            public MyList()
            {
                mItems = new List<T>();
            }

            public RecyclerView.Adapter Adapter
            {
                get { return mAdapter; }
                set { mAdapter = value; }
            }

            public void Add(T item)
            {
                mItems.Add(item);

                if (Adapter != null)
                {
                    Adapter.NotifyItemInserted(Count);
                }
            }

            public void Remove(int position)
            {
                mItems.RemoveAt(position);

                if (Adapter != null)
                {
                    Adapter.NotifyItemRemoved(0);
                }
            }

            public T this[int index]
            {
                get { return mItems[index]; }
                set { mItems[index] = value; }
            }

            public int Count
            {
                get { return mItems.Count; }
            }

            public void clear()
            {
                int size = mItems.Count;
                Erase();
                mAdapter.NotifyItemRangeRemoved(0, size);
            }
        }

        public class RecyclerAdapter : RecyclerView.Adapter
        {
            private MyList<LinkomanijosData> List;
            private RecyclerView mRecyclerView;

            public RecyclerAdapter(MyList<LinkomanijosData> mList, RecyclerView recyclerView)
            {
                List = mList;
                mRecyclerView = recyclerView;
            }

            public class Loading : RecyclerView.ViewHolder
            {
                public View LoadingView { get; set; }

                public Loading(View view) : base(view)
                { }
            }

            public class LinkomanijaData : RecyclerView.ViewHolder
            {
                public View mMoodleViewList { get; set; }
                public TextView mName { get; set; }
                public TextView msubtext { get; set; }
                public TextView mdate { get; set; }
                public TextView msize { get; set; }
                public TextView mdownloaded { get; set; }
                public TextView mseeder { get; set; }

                public LinkomanijaData(View view) : base(view)
                {
                    mMoodleViewList = view;
                }
            }

            public override int GetItemViewType(int position)
            {
                if (List[position] == null)
                {
                    return Resource.Layout.Loading;
                }
                else
                {
                    return Resource.Layout.LinkomanijosList;
                }
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                if (viewType == Resource.Layout.Loading)
                {
                    View Loading = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Loading, parent, false);

                    Loading view = new Loading(Loading) { };

                    return view;
                }
                else
                {
                    View mLinkomanijaListView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.LinkomanijosList, parent, false);

                    TextView name = mLinkomanijaListView.FindViewById<TextView>(Resource.Id.name);
                    TextView subtext = mLinkomanijaListView.FindViewById<TextView>(Resource.Id.subname);
                    TextView date = mLinkomanijaListView.FindViewById<TextView>(Resource.Id.date);
                    TextView size = mLinkomanijaListView.FindViewById<TextView>(Resource.Id.size);
                    TextView downloaded = mLinkomanijaListView.FindViewById<TextView>(Resource.Id.downloaded);
                    TextView seeder = mLinkomanijaListView.FindViewById<TextView>(Resource.Id.seeders);

                    LinkomanijaData view = new LinkomanijaData(mLinkomanijaListView)
                    {
                        mName = name,
                        msubtext = subtext,
                        mdate = date,
                        msize = size,
                        mdownloaded = downloaded,
                        mseeder = seeder
                    };
                    return view;
                }
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                if (holder is Loading)
                {
                    ;
                }
                else
                {
                    LinkomanijaData myHolder = holder as LinkomanijaData;
                    myHolder.mName.Text = List[position].name;
                    myHolder.msubtext.Text = List[position].subtext;
                    myHolder.mdate.Text = List[position].date;
                    myHolder.msize.Text = List[position].size;
                    myHolder.mdownloaded.Text = List[position].downloaded;
                    myHolder.mseeder.Text = List[position].seeder;

                }
            }

            public override int ItemCount
            {
                get { return List.Count; }
            }
        }

        public class RecyclerAdapterTrafi : RecyclerView.Adapter
        {
            private MyList<TrafiListModel> TrafiList;
            private RecyclerView mRecyclerView;
            private Context RVContext;

            public RecyclerAdapterTrafi(MyList<TrafiListModel> mList, RecyclerView recyclerView, Context context)
            {
                TrafiList = mList;
                mRecyclerView = recyclerView;
                RVContext = context;
            }

            public class Loading : RecyclerView.ViewHolder
            {
                public View LoadingView { get; set; }

                public Loading(View view) : base(view)
                { }
            }

            public class TrafiListview : RecyclerView.ViewHolder
            {
                public View ConversationListHolder { get; set; }
                public TextView mEndStreet { get; set; }
                public TextView mEndTime { get; set; }
                public TextView mStartTime { get; set; }
                public TextView mNextStopTime { get; set; }
                public TextView mNextStopDistance { get; set; }
                public TextView mImageBottomDistance { get; set; }
                public ImageView mImage { get; set; }

                public TrafiListview(View view) : base(view)
                {
                    ConversationListHolder = view;
                }
            }

            public override int GetItemViewType(int position)
            {
                if (TrafiList[position] == null)
                {
                    return Resource.Layout.Loading;
                }
                else
                {
                    return Resource.Layout.Trafi;
                }
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                if (viewType == Resource.Layout.Loading)
                {
                    View Loading = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Loading, parent, false);

                    Loading view = new Loading(Loading) { };

                    return view;
                }
                else
                {
                    View TrafiViewList = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Trafi, parent, false);

                    TextView EndStreet = TrafiViewList.FindViewById<TextView>(Resource.Id.EndStreet);
                    TextView EndTime = TrafiViewList.FindViewById<TextView>(Resource.Id.EndTime);
                    TextView StartTime = TrafiViewList.FindViewById<TextView>(Resource.Id.StartTime);
                    TextView NextStopTime = TrafiViewList.FindViewById<TextView>(Resource.Id.NextStopTime);
                    TextView NextStopDistance = TrafiViewList.FindViewById<TextView>(Resource.Id.NextStopDistance);
                    TextView ImageBottomDistance = TrafiViewList.FindViewById<TextView>(Resource.Id.ImageBottomDistance);
                    ImageView Image = TrafiViewList.FindViewById<ImageView>(Resource.Id.Image);

                    TrafiListview view = new TrafiListview(TrafiViewList)
                    {
                        mEndStreet = EndStreet,
                        mEndTime = EndTime,
                        mStartTime = StartTime,
                        mNextStopTime = NextStopTime,
                        mNextStopDistance = NextStopDistance,
                        mImageBottomDistance = ImageBottomDistance,
                        mImage = Image
                    };

                    return view;
                }
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                if (holder is Loading)
                {
                    ;
                }
                else
                {
                    TrafiListview myHolder = holder as TrafiListview;
                    myHolder.mEndStreet.Text = TrafiList[position].EndStreet;
                    myHolder.mEndTime.Text = TrafiList[position].EndTime;
                    myHolder.mStartTime.Text = TrafiList[position].StartTime;
                    myHolder.mNextStopTime.Text = TrafiList[position].NextStopTime;
                    myHolder.mNextStopDistance.Text = TrafiList[position].NextStopDistance;
                    myHolder.mImageBottomDistance.Text = TrafiList[position].ImageBottomDistance;

                    if (!TrafiList[position].Image.Contains("walksegment"))
                    {
                        Picasso.With(RVContext).Load(TrafiList[position].Image).Resize(50, 50).CenterCrop().Into(myHolder.mImage);
                    }

                }
            }

            public override int ItemCount
            {
                get { return TrafiList.Count; }
            }
        }


        public static void Erase()
        {
            for (int i = List.Count; i >= 0; i--)
            {
                if (List.Count == 1)
                {
                    List.Remove(0);
                }
                if (List.Count > 1)
                {
                    List.Remove(List.Count - 1);
                }
            }
            List.Erase();
        }
        public static void EraseCity()
        {
            for (int i = ToCityList.Count; i >= 0; i--)
            {
                if (ToCityList.Count == 1)
                {
                    ToCityList.Remove(0);
                }
                if (ToCityList.Count > 1)
                {
                    ToCityList.Remove(ToCityList.Count - 1);
                }
            }
            ToCityList.Erase();
        }
        public static void EraseGym()
        {
            for (int i = ToGymList.Count; i >= 0; i--)
            {
                if (ToGymList.Count == 1)
                {
                    ToGymList.Remove(0);
                }
                if (ToGymList.Count > 1)
                {
                    ToGymList.Remove(ToGymList.Count - 1);
                }
            }
            ToGymList.Erase();
        }
        #endregion

        #region Speech

        private void RecordVoice(object s, EventArgs e)
        {
            var result = lisenTome;
            lisenTome.Text = string.Empty;
            var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak Now :)");
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            StartActivityForResult(voiceIntent, VOICE);
        }

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            if (requestCode == VOICE)
            {
                if (resultVal == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    lisenTome.Text = matches[0].ToString();
                }
            }
            base.OnActivityResult(requestCode, resultVal, data);
        }

        public void OnInit([GeneratedEnum] OperationResult status)
        {
            if (status == OperationResult.Success)
            {
                tts.SetLanguage(Locale.English);
                Speak();
            }
        }

        private void Speak()
        {
            string text = SpeechText.Text;
            if (!string.IsNullOrEmpty(text))
            {
                tts.Speak(text, QueueMode.Flush, null);
            }
        }


        public void OnBeginningOfSpeech()
        {
            lisenTome.Text = "Beginning";
        }

        public void OnBufferReceived(byte[] buffer)
        {
            lisenTome.Text = "Duomenys analizuojami";
        }

        public void OnEndOfSpeech()
        {
            lisenTome.Text = "Klausausi";
            //audioManager.SetStreamVolume(Stream.Music, musicOrigVol, 0);
        }

        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {
            lisenTome.Text = error.ToString();
            audioManager.SetStreamVolume(Stream.Music, musicOrigVol, 0);
        }

        public void OnEvent(int eventType, Bundle @params)
        {
        }

        public void OnPartialResults(Bundle partialResults)
        {
        }

        public void OnReadyForSpeech(Bundle @params)
        {
            lisenTome.Text = "Ready!";
        }

        public void OnResults(Bundle results)
        {
            var data = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            //    StringBuilder builder = new StringBuilder();
            //    for (int i = 0; i < data.Count; i++)
            //    {
            //        builder.Append(data[i]);
            //    }
            if (data.Any(x => x.Contains("hi")))
            {
                lisenTome.Text = "Hi";
            }
            audioManager.SetStreamVolume(Stream.Music, musicOrigVol, 0);

        }

        public void OnRmsChanged(float rmsdB)
        {
        }
        #endregion
    }
}

