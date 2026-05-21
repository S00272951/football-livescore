using Newtonsoft.Json;
using Project1.Models.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Project1
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Match> _liveMatches = new ObservableCollection<Match>();
        private ObservableCollection<Match> _allFixRes = new ObservableCollection<Match>();
        private ObservableCollection<LeagueTeam> _standings = new ObservableCollection<LeagueTeam>();

        private ICollectionView _liveView;
        private ICollectionView _fixView;

        private string API_KEY = "6c976f25eamsh741f773fbd19b37p1e7380jsn15682b8cad17";

        private DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
        }

        //When the app starts, this method runs
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadLeagueTable();
            await LoadLiveMatches();
            await LoadFixtures();

            _liveView = CollectionViewSource.GetDefaultView(_liveMatches);
            _fixView = CollectionViewSource.GetDefaultView(_allFixRes);

            lstLive.ItemsSource = _liveView;
            lstFixtures.ItemsSource = _fixView;
            dgTable.ItemsSource = _standings;

            var leagues = new List<string> { "All", "Premier League" };

            cmbLiveLeague.ItemsSource = leagues;
            cmbFixLeague.ItemsSource = leagues;

            cmbLiveLeague.SelectedIndex = 0;
            cmbFixLeague.SelectedIndex = 0;

            ApplyLiveFilter();
            ApplyFixFilter();

            StartAutoRefresh();
        }

        //auto refresh using a timer
        private void StartAutoRefresh()
        {
            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += async (s, e) => await LoadLiveMatches();
            timer.Start();
        }

        private async Task LoadLeagueTable()
        {
            try
            {
                //using HttpClient to connect to an online football API
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-rapidapi-key", API_KEY);
                client.DefaultRequestHeaders.Add("x-rapidapi-host", "livescore6.p.rapidapi.com");

                var response = await client.GetAsync("https://livescore6.p.rapidapi.com/competitions/get-table?CompId=65");

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<LiveScoreLeagueResponse>(json);

                _standings.Clear();

                var teams = data.Stages[0].LeagueTable.L[0].Tables[0].team;

                foreach (var t in teams)
                    _standings.Add(t);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Table API Error: " + ex.Message);
            }
        }

        // This method loads live matches from the API, filters for Premier League, and populates the _liveMatches collection
        private async Task LoadLiveMatches()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-rapidapi-key", API_KEY);
                client.DefaultRequestHeaders.Add("x-rapidapi-host", "livescore6.p.rapidapi.com");

                var response = await client.GetAsync("https://livescore6.p.rapidapi.com/matches/v2/list-live");

                var json = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(json);

                _liveMatches.Clear();

                if (data?.Stages == null)
                    return;

                foreach (var stage in data.Stages)
                {
                    if ((string)stage.CompId != "65")
                        continue;

                    if (stage.Events == null)
                        continue;

                    foreach (var match in stage.Events)
                    {
                        string homeName = (string)match.T1[0].Nm;
                        string awayName = (string)match.T2[0].Nm;

                        _liveMatches.Add(new Match
                        {
                            EventId = (string)match.Eid,
                            League = "Premier League",
                            Home = new Team { Name = homeName, Logo = GetTeamLogo(homeName) },
                            Away = new Team { Name = awayName, Logo = GetTeamLogo(awayName) },
                            HomeGoals = (int?)match.Tr1 ?? 0,
                            AwayGoals = (int?)match.Tr2 ?? 0,
                            Minute = (int?)match.Tm ?? 0,
                            Status = MatchStatus.Live,
                            Kickoff = DateTime.Now
                        });
                    }
                }

                if (_liveMatches.Count == 0)
                {
                    g
                    _liveMatches.Add(new Match
                    {
                        League = "Premier League",
                        Home = new Team { Name = "No live EPL matches" },
                        Away = new Team { Name = "-" },
                        Status = MatchStatus.Live,
                        Kickoff = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Live API Error: " + ex.Message);
            }
        }

        // This method loads today's fixtures from the API, filters for Premier League, and populates the _allFixRes collection
        private async Task LoadFixtures()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-rapidapi-key", API_KEY);
                client.DefaultRequestHeaders.Add("x-rapidapi-host", "livescore6.p.rapidapi.com");

                string today = DateTime.Now.ToString("yyyyMMdd");

                var response = await client.GetAsync(
                    $"https://livescore6.p.rapidapi.com/matches/v2/list-by-date?Category=soccer&Date={today}");

                var json = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(json);

                _allFixRes.Clear();

                foreach (var stage in data.Stages)
                {
                    if ((string)stage.Snm != "Premier League")
                        continue;

                    foreach (var match in stage.Events)
                    {
                        string homeName = (string)match.T1[0].Nm;
                        string awayName = (string)match.T2[0].Nm;

                        _allFixRes.Add(new Match
                        {
                            League = "Premier League",
                            Home = new Team { Name = homeName, Logo = GetTeamLogo(homeName) },
                            Away = new Team { Name = awayName, Logo = GetTeamLogo(awayName) },
                            Status = MatchStatus.Fixture,
                            Kickoff = DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fixtures API Error: " + ex.Message);
            }
        }

        //team logos
        private string GetTeamLogo(string teamName) 
        {
            var logos = new Dictionary<string, string>
            {
                { "Arsenal", "https://logos-world.net/wp-content/uploads/2020/05/Arsenal-Logo.png" },
                { "Chelsea", "https://logos-world.net/wp-content/uploads/2020/06/Chelsea-Logo.png" },
                { "Manchester City", "https://logos-world.net/wp-content/uploads/2020/06/Manchester-City-Logo.png" },
                { "Manchester United", "https://logos-world.net/wp-content/uploads/2020/06/Manchester-United-Logo.png" },
                { "Liverpool", "https://logos-world.net/wp-content/uploads/2020/06/Liverpool-Logo.png" },
                { "Tottenham Hotspur", "https://logos-world.net/wp-content/uploads/2020/06/Tottenham-Hotspur-Logo.png" },
                { "Newcastle United", "https://logos-world.net/wp-content/uploads/2020/06/Newcastle-United-Logo.png" },
                { "Aston Villa", "https://logos-world.net/wp-content/uploads/2020/06/Aston-Villa-Logo.png" }
            };

            return logos.ContainsKey(teamName)
                ? logos[teamName]
                : "https://via.placeholder.com/40";
        }

        
        private async void LstMatches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lb = sender as ListBox;
            var m = lb?.SelectedItem as Match;
            if (m == null) return;

            if (m.Status == MatchStatus.Live)
            {
                await LoadMatchEvents(m);
            }
            else
            {
                m.Events.Clear();
                m.Events.Add(new MatchEvent
                {
                    Minute = 0,
                    Description = "⏳ Match not started"
                });
            }

            if (lb.Name == "lstLive")
            {
                txtDetailsTitle.Text = m.DisplayTeams;
                txtDetailsInfo.Text =
                    m.League + "\n" +
                    m.DisplayKickoff + "\n" +
                    "Status: " + m.DisplayStatus + "\n" +
                    "Score: " + m.DisplayScore;

                lstEvents.ItemsSource = m.Events;
                lstEvents.Items.Refresh();
            }
            else if (lb.Name == "lstFixtures")
            {
                txtDetailsTitle2.Text = m.DisplayTeams;
                txtDetailsInfo2.Text =
                    m.League + "\n" +
                    m.DisplayKickoff + "\n" +
                    "Status: " + m.DisplayStatus + "\n" +
                    "Score: " + m.DisplayScore;

                lstEvents2.ItemsSource = m.Events;
                lstEvents2.Items.Refresh();
            }
        }

        // This method loads match events for a given match from the API and populates the match's Events collection
        private async Task LoadMatchEvents(Match match)
        {
            try
            {
                match.Events.Clear();

                if (string.IsNullOrEmpty(match.EventId))
                {
                    match.Events.Add(new MatchEvent { Minute = 0, Description = "No events available" });
                    return;
                }

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-rapidapi-key", API_KEY);
                client.DefaultRequestHeaders.Add("x-rapidapi-host", "livescore6.p.rapidapi.com");

                var response = await client.GetAsync(
                    $"https://livescore6.p.rapidapi.com/matches/v2/get-details?Eid={match.EventId}");

                var json = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(json);

                var events = data?.Stages?[0]?.Events?[0]?.E;

                if (events != null)
                {
                    foreach (var ev in events)
                    {
                        string desc = (string)ev.Des ?? "Event";

                        if (desc.ToLower().Contains("goal"))
                            desc = "⚽ " + desc;
                        else if (desc.ToLower().Contains("yellow"))
                            desc = "🟨 " + desc;
                        else if (desc.ToLower().Contains("red"))
                            desc = "🟥 " + desc;

                        match.Events.Add(new MatchEvent
                        {
                            Minute = (int?)ev.Min ?? 0,
                            Description = desc
                        });
                    }
                }

                //If there are no events, I show a message so the UI is never empty
                if (match.Events.Count == 0)
                {
                    match.Events.Add(new MatchEvent
                    {
                        Minute = 0,
                        Description = "No events yet"
                    });
                }
            }
            catch
            {
                match.Events.Clear();
                match.Events.Add(new MatchEvent
                {
                    Minute = 0,
                    Description = "Error loading events"
                });
            }
        }

        //button click handlers to switch between tabs and exit
        private void BtnGoLive_Click(object sender, RoutedEventArgs e) => tabMain.SelectedIndex = 1;
        private void BtnGoFixtures_Click(object sender, RoutedEventArgs e) => tabMain.SelectedIndex = 2;
        private void BtnBackToMenu_Click(object sender, RoutedEventArgs e) => tabMain.SelectedIndex = 0;
        private void BtnExit_Click(object sender, RoutedEventArgs e) => Close();


        //filtering logic for live matches and fixtures based on league and search query
        private void ApplyLiveFilter()
        {
            if (_liveView == null) return;

            string league = cmbLiveLeague.SelectedItem?.ToString() ?? "All";
            string q = txtLiveSearch.Text?.ToLower() ?? "";

            _liveView.Filter = obj =>
            {
                var m = obj as Match;
                return (league == "All" || m.League == league) &&
                       (q == "" || m.Home.Name.ToLower().Contains(q) || m.Away.Name.ToLower().Contains(q));
            };

            _liveView.Refresh();
        }

        private void ApplyFixFilter()
        {
            if (_fixView == null) return;

            bool showFixtures = rbFixtures.IsChecked == true;
            MatchStatus status = showFixtures ? MatchStatus.Fixture : MatchStatus.Finished;

            string league = cmbFixLeague.SelectedItem?.ToString() ?? "All";
            string q = txtFixSearch.Text?.ToLower() ?? "";

            _fixView.Filter = obj =>
            {
                var m = obj as Match;
                return m.Status == status &&
                       (league == "All" || m.League == league) &&
                       (q == "" || m.Home.Name.ToLower().Contains(q) || m.Away.Name.ToLower().Contains(q));
            };

            _fixView.Refresh();
        }

        // Event handlers to trigger filtering when the user changes the league selection or search text
        private void CmbLiveLeague_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyLiveFilter();
        private void TxtLiveSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyLiveFilter();
        private void CmbFixLeague_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFixFilter();
        private void TxtFixSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFixFilter();
        private void RbMode_Checked(object sender, RoutedEventArgs e) => ApplyFixFilter();
    }
}