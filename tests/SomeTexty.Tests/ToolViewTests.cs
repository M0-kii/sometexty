using System.Windows.Controls;
using System.Windows.Threading;
using SomeTexty.Autocorrect;
using SomeTexty.CaseConversion;
using SomeTexty.Tools;
using Xunit;

namespace SomeTexty.Tests;

public class ToolViewTests
{
    private static readonly Lazy<Dispatcher> UiDispatcher = new(StartDispatcher);

    [Fact]
    public void Case_clipboard_autofill_keeps_existing_edits()
    {
        UiDispatcher.Value.Invoke(() =>
        {
            var view = new CaseView();
            var input = (TextBox)view.FindName("Input");
            IClipboardAware tool = view;

            tool.OfferClipboardText("clipboard text");
            Assert.Equal("clipboard text", input.Text);

            input.Text = "my edits";
            tool.OfferClipboardText("new clipboard text");
            Assert.Equal("my edits", input.Text);
        });
    }

    [Fact]
    public async Task Autocorrect_accepts_edits_while_the_dictionary_loads()
    {
        await UiDispatcher.Value.InvokeAsync(async () =>
        {
            var loading = new TaskCompletionSource<SpellEngine>();
            var view = new AutocorrectView(loading.Task);
            var input = (TextBox)view.FindName("Input");
            var fixAll = (Button)view.FindName("FixAll");

            Assert.False(fixAll.IsEnabled);
            view.OfferClipboardText("teh");
            input.Text = "recieve";

            loading.SetResult(SpellEngine.Instance);
            await Dispatcher.Yield(DispatcherPriority.ApplicationIdle);

            var issues = (WrapPanel)view.FindName("Issues");
            var chip = Assert.IsType<Button>(Assert.Single(issues.Children.Cast<object>()));
            Assert.Equal("recieve", chip.Content);
            Assert.True(fixAll.IsEnabled);

            fixAll.RaiseEvent(new System.Windows.RoutedEventArgs(Button.ClickEvent));
            Assert.Equal("receive", input.Text);
        }).Task.Unwrap();
    }

    [Fact]
    public void Autocorrect_stays_usable_if_the_dictionary_cannot_load()
    {
        UiDispatcher.Value.Invoke(() =>
        {
            var view = new AutocorrectView(
                Task.FromException<SpellEngine>(new InvalidOperationException("Missing dictionary")));
            view.OfferClipboardText("some text");

            var status = (TextBlock)view.FindName("Status");
            Assert.Equal("Couldn't load the spelling dictionary.", status.Text);
            Assert.False(((Button)view.FindName("FixAll")).IsEnabled);
            Assert.True(((Button)view.FindName("Copy")).IsEnabled);
        });
    }

    private static Dispatcher StartDispatcher()
    {
        var ready = new TaskCompletionSource<Dispatcher>();
        var thread = new Thread(() =>
        {
            try
            {
                var app = new System.Windows.Application
                {
                    ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown,
                };
                app.Resources.Add("ActionButton", new System.Windows.Style(typeof(Button)));
                var dispatcher = Dispatcher.CurrentDispatcher;
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(dispatcher));
                ready.SetResult(dispatcher);
                Dispatcher.Run();
            }
            catch (Exception error)
            {
                ready.TrySetException(error);
            }
        }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return ready.Task.GetAwaiter().GetResult();
    }
}
