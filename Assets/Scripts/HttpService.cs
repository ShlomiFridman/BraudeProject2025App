using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace ProjectClasses
{
    
    public class HttpService
    {
        public static HttpService Instance = new();
        
        private readonly HttpClient _client;
        private readonly string url;

        private HttpService()
        {
            _client = new HttpClient();
            url = AppConstants.IsLocalServer?
                    $"http://{AppConstants.ServerLocalAddress}" :
                    $"https://{AppConstants.ServerDeploymentAddress}";
        }

        public async Task<bool> PostQuiz(QuizMessage quizMessage)
        {
            var json = JsonConvert.SerializeObject(quizMessage);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response  = await _client.PostAsync($"{url}/user/addQuizResult", content);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                Debug.Log($"PostQuiz :: ResponseCode: {response.StatusCode}, body: {responseBody}");

                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"PostQuiz :: Exception: {e.Message}");
                return false;
            }
        }

        public async Task<List<Badge>> GetBadges()
        {
            var getUrl = $"{url}/data/getBadges";
            try
            {
                var response = await _client.GetAsync(getUrl);
                response.EnsureSuccessStatusCode();
                
                var responseBody = await response.Content.ReadAsStringAsync();
                var badges = JsonConvert.DeserializeObject<List<Badge>>(responseBody);
                return badges;
            }
            catch (Exception e)
            {
                Debug.Log($"GetBadges :: Exception: {e.Message}");
                return null;
            }
        }

        public async Task<List<UserBadge>> GetUserBadges(string username)
        {
            var uriBuilder = new UriBuilder($"{url}/user/getUserBadges/{username}");
            // uriBuilder.Query = $"username={username}";
            try
            {
                var response = await _client.GetAsync(uriBuilder.Uri);
                response.EnsureSuccessStatusCode();
                
                var responseBody = await response.Content.ReadAsStringAsync();
                var badges = JsonConvert.DeserializeObject<List<UserBadge>>(responseBody);
                return badges;
            }
            catch (Exception e)
            {
                Debug.Log($"GetUserBadges :: Exception: {e.Message}");
                return null;
            }
        }

        public async Task<Quiz> GetQuiz(int groupNumber)
        {
            var uriBuilder = new UriBuilder($"{url}/data/getQuiz/{groupNumber}");
            try
            {
                var response = await _client.GetAsync(uriBuilder.Uri);
                response.EnsureSuccessStatusCode();
                
                var responseBody = await response.Content.ReadAsStringAsync();
                var quiz = JsonConvert.DeserializeObject<Quiz>(responseBody);
                return quiz;
            }
            catch (Exception e)
            {
                Debug.Log($"GetQUiz (groupNumber={groupNumber}) :: Exception: {e.Message}");
                return null;
            }
        }

        public async Task<UserProgress> GetProgress(string username)
        {
            var uriBuilder = new UriBuilder($"{url}/user/getProgress/{username}");
            try
            {
                var response = await _client.GetAsync(uriBuilder.Uri);
                response.EnsureSuccessStatusCode();
                
                var responseBody = await response.Content.ReadAsStringAsync();
                var progress = JsonConvert.DeserializeObject<UserProgress>(responseBody);
                return progress;
            }
            catch (Exception e)
            {
                Debug.Log($"GetProgress (username={username}) :: Exception: {e.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateProgress(UserProgress progress)
        {
            var json = JsonConvert.SerializeObject(progress);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response  = await _client.PutAsync($"{url}/user/updateProgress", content);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                Debug.Log($"UpdateProgress :: ResponseCode: {response.StatusCode}, body: {responseBody}");

                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"UpdateProgress :: Exception: {e.Message}");
                return false;
            }
        }

        public async Task<bool> AddBadge(string username, int badge_id)
        {
            var msgObj = new AddBadgeMessage(username, badge_id);
            var json = JsonConvert.SerializeObject(msgObj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response  = await _client.PostAsync($"{url}/user/addBadge", content);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                Debug.Log($"AddBadge :: ResponseCode: {response.StatusCode}, body: {responseBody}");

                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"AddBadge :: Exception: {e.Message}");
                return false;
            }
        }
    }
}