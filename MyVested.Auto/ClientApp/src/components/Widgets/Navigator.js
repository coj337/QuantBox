import React, { Component } from 'react';
import Tree from 'react-animated-tree';
import { Widget } from './Widget';

export class Navigator extends Component {
    render() {
        return (
            <div>
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
                    <Tree content="Tax Calculator" />
                </Tree>
            </div>
        );
    }
}