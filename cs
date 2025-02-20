using cAlgo.API;
using System;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class TradingViewWebView : Robot
    {
        private WebView _webView;
        private Button _reloadButton;
        private TextBox _symbolTextBox;
        private Grid mainGrid;
        private CheckBox _visibilityCheckBox;

        protected override void OnStart()
        {
            var controlPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                BackgroundColor = Color.Black,
                Margin = 3,
                Width = 300,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            _symbolTextBox = new TextBox
            {
                Text = "FXOPEN:XAUUSD",
                Width = 150,
                Margin = 3,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            _reloadButton = new Button
            {
                Text = "Reload Chart",
                Width = 50,
                Margin = 3,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            _visibilityCheckBox = new CheckBox
            {
                Text = "Show",
                IsChecked = true,
                Margin = 3,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            _reloadButton.Click += OnReloadButtonClick;
            _visibilityCheckBox.Checked += OnVisibilityChanged;
            _visibilityCheckBox.Unchecked += OnVisibilityChanged;

            controlPanel.AddChild(_symbolTextBox);
            controlPanel.AddChild(_reloadButton);
            controlPanel.AddChild(_visibilityCheckBox);

            mainGrid = new Grid(1, 1)
            {
                Width = 200,
                Height = 500,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 3, 3, 0)
            };

            _webView = new WebView
            {
                DefaultBackgroundColor = Color.Black,
                Width = 350,
                Height = 500,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            _webView.Loaded += OnWebViewLoaded;
            _webView.NavigationCompleted += OnWebViewNavigationCompleted;

            var mainStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            mainStackPanel.AddChild(controlPanel);
            mainGrid.AddChild(_webView, 0, 0);
            mainStackPanel.AddChild(mainGrid);

            Chart.AddControl(mainStackPanel);
            LoadTradingViewChart();
        }

        private void OnVisibilityChanged(CheckBoxEventArgs obj)
        {
            mainGrid.IsVisible = obj.CheckBox.IsChecked ?? false;
        }

        private void LoadTradingViewChart()
        {
            var tradingViewWidgetHtml = $@"
    <div class=""tradingview-widget-container"" style=""height:100%;width:100%"">
        <div class=""tradingview-widget-container__widget"" style=""height:100%;width:100%""></div>
        <script type=""text/javascript"" src=""https://s3.tradingview.com/external-embedding/embed-widget-advanced-chart.js"" async>
        {{
            ""autosize"": true,
            ""symbol"": ""{_symbolTextBox.Text}"",
            ""interval"": ""1"",
            ""timezone"": ""Asia/Tehran"",
            ""theme"": ""dark"",
            ""style"": ""1"",
            ""locale"": ""en"",
            ""withdateranges"": true,
            ""backgroundColor"": ""rgba(0, 0, 0, 1)"",
            ""gridColor"": ""rgba(0, 0, 0, 1)"",
            ""allow_symbol_change"": true,
            ""barcolor_change"": true,
            ""calendar"": true,
            ""hide_side_toolbar"": false,
            ""hide_volume"": true,
            ""support_host"": ""https://www.tradingview.com""
        }}
        </script>
    </div>";

            _webView.NavigateToStringAsync(tradingViewWidgetHtml);
        }

        private void OnReloadButtonClick(ButtonClickEventArgs args)
        {
            LoadTradingViewChart();
        }

        private void OnWebViewLoaded(WebViewLoadedEventArgs args)
        {
            Print("WebView loaded successfully");
        }

        private void OnWebViewNavigationCompleted(WebViewNavigationCompletedEventArgs args)
        {
            Print($"Navigation completed: {args.Url}, Status: {args.HttpStatusCode}");
        }

        protected override void OnStop()
        {
            Chart.RemoveControl(mainGrid);
        }
    }
}
