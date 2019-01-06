import React, { Component } from 'react';
import Tree from 'react-animated-tree';
import { Widget } from './Widget';

export class Navigator extends Component {
    render() {
        return (
            <Widget
                title={"Navigator"}
                minH={2}
                minW={2}
                currentH={2}
                currentW={2}
            >
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
            </Widget>
        );
    }
}