using EventEase.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;

namespace EventEase.Services
{
    public class SessionStateService
    {
        private readonly ProtectedLocalStorage _localStorage;
        private readonly ILogger<SessionStateService> _logger;
        
        // 当前会话的用户
        private User? _currentUser;
        public User? CurrentUser 
        { 
            get => _currentUser;
            private set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    // 当用户改变时触发事件
                    OnUserSessionChanged(new UserSessionChangedEventArgs(_currentUser, 
                        _currentUser != null ? "LoggedIn" : "LoggedOut"));
                }
            }
        }
        
        // 会话开始时间
        public DateTime SessionStartTime { get; private set; }
        
        // 最后活动时间
        public DateTime LastActivityTime { get; private set; }
        
        // 会话超时时间（分钟）
        private const int SessionTimeoutMinutes = 30;
        
        // 事件：当用户状态改变时触发
        public event EventHandler<UserSessionChangedEventArgs>? UserSessionChanged;
        
        public SessionStateService(ProtectedLocalStorage localStorage, ILogger<SessionStateService> logger)
        {
            _localStorage = localStorage;
            _logger = logger;
        }
        
        // 初始化会话 - 在应用启动时调用
        public async Task InitializeAsync()
        {
            await RestoreSessionFromStorage();
            SessionStartTime = DateTime.Now;
            LastActivityTime = DateTime.Now;
            
            _logger.LogInformation("Session initialized. CurrentUser: {User}", 
                CurrentUser?.Username ?? "null");
        }
        
        // 用户登录
        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                _logger.LogInformation("Login attempt for user: {Username}", username);
                
                // 在实际应用中，这里应该验证用户名和密码
                // 这里使用模拟验证
                if (username == "demo" && password == "password")
                {
                    CurrentUser = new User
                    {
                        Id = 1,
                        Username = username,
                        Email = "demo@example.com",
                        FullName = "Demo User",
                        LastLoginTime = DateTime.Now,
                        IsActive = true
                    };
                    
                    SessionStartTime = DateTime.Now;
                    LastActivityTime = DateTime.Now;
                    
                    _logger.LogInformation("Login successful for user: {Username}", username);
                    
                    // 保存到本地存储
                    await SaveSessionToStorage();
                    
                    return true;
                }
                
                _logger.LogWarning("Login failed for user: {Username}", username);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return false;
            }
        }
        
        // 用户登出
        public async Task LogoutAsync()
        {
            _logger.LogInformation("User logged out: {Username}", CurrentUser?.Username);
            CurrentUser = null;
            await ClearSessionFromStorage();
        }
        
        // 更新活动时间
        public void UpdateActivity()
        {
            LastActivityTime = DateTime.Now;
        }
        
        // 检查会话是否有效
        public bool IsSessionValid()
        {
            if (CurrentUser == null)
                return false;
                
            // 检查是否超时
            if ((DateTime.Now - LastActivityTime).TotalMinutes > SessionTimeoutMinutes)
            {
                _logger.LogInformation("Session timeout for user: {Username}", CurrentUser.Username);
                _ = Task.Run(async () => await LogoutAsync()); // 异步登出
                return false;
            }
            
            return true;
        }
        
        // 保存会话到本地存储
        private async Task SaveSessionToStorage()
        {
            if (CurrentUser != null)
            {
                var sessionData = new SessionData
                {
                    User = CurrentUser,
                    SessionStartTime = SessionStartTime,
                    LastActivityTime = LastActivityTime
                };
                
                var json = JsonSerializer.Serialize(sessionData);
                await _localStorage.SetAsync("userSession", json);
                _logger.LogDebug("Session saved to storage for user: {Username}", CurrentUser.Username);
            }
        }
        
        // 从本地存储恢复会话
        public async Task<bool> RestoreSessionFromStorage()
        {
            try
            {
                var result = await _localStorage.GetAsync<string>("userSession");
                if (result.Success && !string.IsNullOrEmpty(result.Value))
                {
                    var sessionData = JsonSerializer.Deserialize<SessionData>(result.Value);
                    if (sessionData != null && sessionData.User != null)
                    {
                        CurrentUser = sessionData.User;
                        SessionStartTime = sessionData.SessionStartTime;
                        LastActivityTime = sessionData.LastActivityTime;
                        
                        _logger.LogInformation("Session restored for user: {Username}", CurrentUser?.Username);
                        
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring session");
            }
            
            return false;
        }
        
        // 清除会话
        private async Task ClearSessionFromStorage()
        {
            await _localStorage.DeleteAsync("userSession");
            _logger.LogDebug("Session cleared from storage");
        }
        
        private void OnUserSessionChanged(UserSessionChangedEventArgs e)
        {
            _logger.LogDebug("User session changed: {Action}", e.Action);
            UserSessionChanged?.Invoke(this, e);
        }
        
        // 内部类用于序列化会话数据
        private class SessionData
        {
            public User? User { get; set; }
            public DateTime SessionStartTime { get; set; }
            public DateTime LastActivityTime { get; set; }
        }
    }
    
    // 会话改变事件参数
    public class UserSessionChangedEventArgs : EventArgs
    {
        public User? User { get; }
        public string Action { get; }
        
        public UserSessionChangedEventArgs(User? user, string action)
        {
            User = user;
            Action = action;
        }
    }
}