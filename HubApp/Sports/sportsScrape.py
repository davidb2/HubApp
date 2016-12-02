from bs4 import BeautifulSoup
import urllib2
import json

# returns json as dictionary with key sport name ("NFL" or "NBA")
# each sport has array of games
# each game has array of teams playing
# each team is a dictionary with keys "score", "icon", "name"
# field "icon" is an internet url to a 200x200 icon for the team


def getGamesJSON():
    urls = {'NBA': 'http://www.foxsports.com/nba/schedule', 'NFL': 'http://www.foxsports.com/nfl/schedule'}
    all_sports_json = {}
    for url in urls:
        html = urllib2.urlopen(urls[url]).read()
        soup = BeautifulSoup(html, 'html.parser')
        games = []
        games_table = soup.find('table', {'class': 'wisbb_scheduleTable'}).find('tbody').find_all('tr')
        for game in games_table:
            game_teams = []
            for team in game.find_all('td', {'class': 'wisbb_team'}):
                score = team.find('div', {'class': 'wisbb_score'}).text
                icon = team.find('img', {'class': 'wisfb_logoImage'})['src']
                name = team.find('img', {'class': 'wisfb_logoImage'})['alt']
                game_teams.append({'score': score, 'icon': icon, 'name': name})
            games.append(game_teams)
        all_sports_json[url] = games
    return json.dump(all_sports_json)
