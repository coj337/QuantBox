import React, { Component } from 'react';
import Tree from 'react-animated-tree';

export class Navigator extends Component {
    render() {
        return (
            <span>
                <div className="widgetHandle">Navigator</div>
                <div className="widgetBody">
                    <Tree content="Accounts">
                        <Tree content="Poloniex" />
                        <Tree content="Bittrex" />
                        <Tree content="Binance" />
                    </Tree>
                    <Tree content="Indicators">
                        <Tree content="RSI" />
                        <Tree content="MACD" />
                    </Tree>
                    <Tree content="Bots">
                        <Tree content="Breakout" />
                        <Tree content="S&R" />
                        <Tree content="300MA" />
                        <Tree content="Custom" />
                    </Tree>
                    <Tree content="Scripts">
                        <Tree content="Tax Calculator"/>
                    </Tree>
                </div>
            </span>
        );
    }
}