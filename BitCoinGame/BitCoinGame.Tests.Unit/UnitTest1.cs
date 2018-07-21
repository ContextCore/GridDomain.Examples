using System;
using Xunit;

namespace BitCoinGame.Tests.Unit
{
    /// <summary>
    /// Bitcoin game is a stake-based game when player has initial funds 
    /// and should increase it to given amount
    /// 
    /// Player can bid on bitcoin price raise or low with amount of money 
    /// Amount is reduced from player funds
    /// 
    /// After a time player can check his bid 
    /// 
    /// If he won, double bid is returned to player balance 
    /// 
    /// If he lost, bid is not returned 
    /// 
    /// If player has zero funds, he loose 
    /// If player increase his funds to target amount, he won
    /// Player can not place a bid for more money that he has
    /// Player can have only one active bid 
    
    /// </summary>
    public class BitCoinGameTests
    {
        [Fact]
        public void Given_Game_When_bid_is_placed_Then_it_amount_is_substracted_from_funds()
        {

        }
        
        [Fact]
        public void Given_Game_When_placing_bid_bigger_then_amount_Then_bid_is_not_accepted()
        {
        }

        [Fact]
        public void Given_Game_When_dib_won_And_total_funds_greater_target_Then_game_is_won()
        {
        }
        [Fact]
        public void Given_Game_When_dib_lost_And_total_funds_are_zero_Then_game_is_lost()
        {
        }

        [Fact]
        public void Given_game_with_active_bid_for_raise_When_bid_is_checked_And_won_Then_double_bid_returned_to_funds()
        {
            
        }
        
        [Fact]
        public void Given_game_with_active_bid_for_lower_When_bid_is_checked_And_won_Then_double_bid_returned_to_funds()
        {
            
        }
        [Fact]
        public void Given_game_with_active_bid_for_lower_When_bid_is_checked_And_lst_Then_funds_not_changed()
        {
            
        }
        
        [Fact]
        public void Given_game_with_active_bid_for_raise_When_bid_is_checked_And_lst_Then_funds_not_changed()
        {
            
        }
    }
}