using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;

public static class Jenna {
	
	protected readonly SpeechRecognitionEngine _recognizer;
	protected readonly ManualResetEvent _completed; 
	protected readonly SpeechSynthesizer _synthesizer;
	
	Jenna() {
	
    	 _completed = new ManualResetEvent(false);
		_recognizer = new SpeechRecognitionEngine();
		_synthesizer = new SpeechSynthesizer();
		
		_recognizer.RequestRecognizerUpdate();
		_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("test")) { Name = "testGrammar" });
		
		_recognizer.SpeechRecognized += _recognizer_SpeechRecognized; 
		_recognizer.SpeechRecognitonRejected += _recognizer_SpeechRecognitionRejected;
		
		_recognizer.SetInputToDefaultAudioDevice();
		
		_recognizer.RecognizeAsync(RecognizeMode.Multiple);
		
	     _completed.WaitOne(); // wait until speech recognition is completed
	     _recognizer.Dispose(); // dispose the speech recognition engine
	     
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
	     	_recognizer.Dispose();
	     	_synthesizer.Dispose();
	        _completed.Set();
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
	
}
