using hexapp_api_cs.Models.Authentication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hexapp_api_cs.Services.Authentication
{
    public class TokenService
    {
        private readonly HexContext _context;

        public TokenService()
        {
            _context = new HexContext();
        }

        public async Task<List<Token>> Get() =>
            await _context.Tokens.ToListAsync();

        public async Task<Token> Get(int id) =>
            await _context.Tokens.Where<Token>(e => e.TokenId == id).FirstOrDefaultAsync();

        public async Task<Token> GetByToken(string token) =>
            await _context.Tokens.Where<Token>(e => e.TokenString == token.ToLower()).FirstOrDefaultAsync();

        public async Task<Token> Create(Token token)
        {
            await _context.Tokens.AddAsync(token);
            await _context.SaveChangesAsync();

            return token;
        }

        public bool IsTokenValid(string tokenString)
        {
            var token = _context.Tokens.Where<Token>(e => e.TokenString == tokenString.ToLower()).FirstOrDefault();

            return (bool)token.ValidFlag;
        }

        public async Task<bool> IsTokenValidAsync(string tokenString)
        {
            var token = await _context.Tokens.Where<Token>(e => e.TokenString == tokenString.ToLower()).FirstOrDefaultAsync();

            return (bool)token.ValidFlag;
        }

        public async Task InvalidateUserTokens(int userId)
        {
            var tokens = await _context.Tokens.Where(e => e.UserId == userId).ToListAsync();

            foreach(Token token in tokens)
            {
                token.ValidFlag = false;
            }

            await _context.SaveChangesAsync();
        }

        public async Task InvalidateToken(int id)
        {
            var token = await _context.Tokens.Where(e => e.TokenId == id).FirstOrDefaultAsync();

            token.ValidFlag = false;

            await _context.SaveChangesAsync();
        }

        public async Task UseToken(int id)
        {
            var token = await _context.Tokens.Where(e => e.TokenId == id).FirstOrDefaultAsync();

            token.UsedFlag = true;

            await _context.SaveChangesAsync();
        }
    }
}
