using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;

public static class Person {

    protected readonly SpeechRecognitionEngine _recognizer;
	protected readonly ManualResetEvent _completed; 
	protected readonly SpeechSynthesizer _synthesizer;

	public bool listening = false;
	public SpeechReconitionResult ReadResult { get; set; }

	/// GRAMMARS

	protected Grammar grammarTest;
	protected Grammar grammarAnimals;
	protected DictationGrammar grammarDictation;

	public Person() {

    	 _completed = new ManualResetEvent(false);
		_recognizer = new SpeechRecognitionEngine();
		_synthesizer = new SpeechSynthesizer();

		InitializeGrammars();

		_recognizer.RequestRecognizerUpdate();
		_recognizer.LoadGrammar( grammarTest );

		_recognizer.SpeechRecognized += _recognizer_SpeechRecognized;
		_recognizer.SpeechRecognitonRejected += _recognizer_SpeechRecognitionRejected;

		_recognizer.SetInputToDefaultAudioDevice();

		_recognizer.RecognizeAsync(RecognizeMode.Multiple);
		listening = true;

	     _completed.WaitOne(); // wait until speech recognition is completed
	     _recognizer.Dispose(); // dispose the speech recognition engine

	}

	void InitializeGrammars() {

		grammarTest = new Grammar(new GrammarBuilder("test")) { Name = "testGrammar" };

		grammarAnimals = new Grammar(new GrammarBuilder(new Choices("dog","cat","snake"))) { Name = "animalGrammar" };

		grammarDictation = new DictationGrammar();

	}

	void SetTolerances() {

        _recognizer.BabbleTimeout = TimeSpan.FromSeconds(10.0);
        _recognizer.EndSilenceTimeout = TimeSpan.FromSeconds(10.0);
        _recognizer.EndSilenceTimeoutAmbiguous = TimeSpan.FromSeconds(10.0);
        _recognizer.InitialSilenceTimeout = TimeSpan.FromSeconds(10.0);
            
	}

	void Say( string msg ) {

		 Console.WriteLine( msg );
		 _synthesizer.Speak( msg );

	}

	void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)	{
		if (e.Result.Text == "test") {
			Say("The test was successful!");
		} else 
		if (e.Result.Text == "exit") {
			_completed.Set();
			Dispose();
		}
	} 

	void _recognizer_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e) {
	   if (e.Result.Alternates.Count == 0){
	     Say("Speech rejected. No candidate phrases found.");
	     return;
	   }
	   Say("Speech rejected. Did you mean:");
	   foreach (RecognizedPhrase r in e.Result.Alternates){
	    Say("    " + r.Text);
	   }
	}

	void Dispose() {

		_recognizer.RecognizeAsyncStop();
		_recognizer.UnloadAllGrammar();
     	_recognizer.Dispose();
     	_synthesizer.Dispose();

	}

}
