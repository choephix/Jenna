using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;

public static class Jenna {

	Jenna() {
	
    	 _completed = new ManualResetEvent(false);
     	
		SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine()
		
		_recognizer.RequestRecognizerUpdate();
		_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("test")) { Name = "testGrammar" });
		
		_recognizer.SpeechRecognized += _recognizer_SpeechRecognized; 
		
		_recognizer.SetInputToDefaultAudioDevice();
		
		_recognizer.RecognizeAsync(RecognizeMode.Multiple);
		
	     _completed.WaitOne(); // wait until speech recognition is completed
	     _recognizer.Dispose(); // dispose the speech recognition engine
	     
	}
	
	void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)	{
	     if (e.Result.Text == "test") {
	         Console.WriteLine("The test was successful!");
	     } else 
	     if (e.Result.Text == "exit") {
	         _completed.Set();
	     }
	} 
	
}
