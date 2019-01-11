# QuantBox

The purpose of QuantBox is to be a easy to use, all in one microservices based platform for cryptocurrency traders, think of it as [Coinigy](https://www.coinigy.com/) x [TradingView](https://www.tradingview.com/) x [Haasbot](https://www.haasonline.com/) x [MetaTrader](https://www.metatrader5.com/en).

With QuantBox the aim is to be able to do anything you want from setting up your trading desk exactly how you want to automating your existing strategy without writing a line of code.

## Features
There's many plans for this and some are sure to get dropped and some will be added, ***suggestions are welcome!***  
**NOTE: Most of this hasn't been made*** (e.g. Multiple dashboards) ***or is a naive implementation** (e.g. Sentiment Analysis)  

### Dashboards!
  - Create as many dashboards as you want
  - Dashboards done your way with draggable, resizable widgets
  - Customize your color scheme, save your layouts
  
### Bots!
The aim is to have a Kano-esque puzzle system to help build bots, everyone loves puzzles. Imagine this but with bot stuff.  
*(Image is taken from a Kano activity)*  
![Kano Image](https://user-images.githubusercontent.com/9269226/51034064-7fff7980-15f9-11e9-8ac5-10bd57c0fef8.png)
  
### Sentiment Analysis!
Sentiment Analysis is useful for manual or automated trading but it's hard, for the time being I'm utilizing the [Vader](https://github.com/cjhutto/vaderSentiment) library but a more trading-focused AI based classifier is in the works.
 - Twitter support (Only one working)
 - Facebook support
 - Intagram support (Who knows, it could be cool!)
 - Reddit support
 - Quora support
 - LinkedIn support
 - Suggest some more!
 
### Arbitrage!
  - Triangle arbitrage (Single exchange and inter-exchange)
  - Two-way arbitrage
  - Configurable blockchain and/or exchange fee calculation
  - Time-per-loop calculation
  - API-less exchange support (Exchanges without an API are much more likely to allow arbitrage!)
  
### Portfolio Tracking!
Lots of pretty charts, I promise.
  - Calculate PNL based off:
    - Trade and price history on all exchanges
    - On individual exchanges
    - For individual coins
    - A mix of anything in between
  - Tax calculations (Tax is hard and I'm not an accountant, don't take this as financial advice etc.) 
  - Stats! Stats! Stats! 
  - Maybe portfolio management (e.g. Automatic re-balancing)

### Exchange Support
Supporting a million exchanges isn't very high on the list while this is a small project but I'll work on requested exchanges as a priority.
  - Binance  
  Next: BtcMarkets
