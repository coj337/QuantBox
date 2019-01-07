import React, { Component } from 'react';
import { Widget } from './Widget';

export class RecentTrades extends Component {
    render() {
        return (
            <Widget
                title={"Recent Trades"}
                minH={2}
                minW={2}
                currentH={2}
                currentW={2}
                x={0}
                y={0}
            >
                Recent Trades
            </Widget>
        );
    }
}