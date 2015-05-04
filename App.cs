using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Input;
using Xamarin.Forms;

namespace DownloadTasksTest
{
	public class App : Application
	{
		Label label1;
		Label label2;
		Label label3;
		Label label4;
		bool IsRunning;

		public App ()
		{
			label1 = new Label {
				XAlign = TextAlignment.Center,
			};
			label2 = new Label {
				XAlign = TextAlignment.Center,
			};
			label3 = new Label {
				XAlign = TextAlignment.Center,
			};
			label4 = new Label {
				XAlign = TextAlignment.Center,
			};

			var mainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Children = {
						new Button{
							Text = "Go!",
							Command = DoTestsCommand,
						},
						label1,
						label2,
						label3,
						label4,
					}
				}
			};

			MainPage = mainPage;
		}

		public ICommand DoTestsCommand {
			get {
				return new Command ((o) => {
					DoTests();
				}, (o) => { return !IsRunning; });
			}
		}

		static TimeSpan TimeDataflowComputations(int maxParallels, int messageCount)
		{
			var workerBlock = new ActionBlock<int> (
				timeout => Task.Delay (timeout),
				new ExecutionDataflowBlockOptions {
					MaxDegreeOfParallelism = maxParallels,
				});

			var stopwatch = new Stopwatch ();
			stopwatch.Start ();

			for (int i = 0; i < messageCount; i++) {
				workerBlock.Post (100);
			}
			workerBlock.Complete ();

			workerBlock.Completion.Wait ();

			stopwatch.Stop ();
			return stopwatch.Elapsed;
		}

		void DoTests()
		{
			IsRunning = true;

			var processorCount = Environment.ProcessorCount;
			var messageCount = 16;

			Debug.WriteLine ("Processors: {0}", processorCount);

			TimeSpan elapsed;

			elapsed = TimeDataflowComputations (1, messageCount);
			label1.Text = String.Format ("P {0}, M {1}, ∂ {2}ms", 1, messageCount, (int)elapsed.TotalMilliseconds);

			elapsed = TimeDataflowComputations (processorCount, messageCount);
			label2.Text = String.Format ("P {0}, M {1}, ∂ {2}ms", messageCount, messageCount, (int)elapsed.TotalMilliseconds);

			elapsed = TimeDataflowComputations (processorCount, messageCount);
			label3.Text = String.Format ("P {0}, M {1}, ∂ {2}ms", processorCount, messageCount, (int)elapsed.TotalMilliseconds);

			elapsed = TimeDataflowComputations (processorCount, messageCount);
			label4.Text = String.Format ("P {0}, M {1}, ∂ {2}ms", DataflowBlockOptions.Unbounded, messageCount, (int)elapsed.TotalMilliseconds);

			IsRunning = false;
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

