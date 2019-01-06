import React, { Component } from 'react';
import { Widget } from './Widget';

export class PriceChart extends Component {
    render() {     
        return (
            <Widget
                title={"Chart (BTC/USD)"}
                minH={2}
                minW={2}
                currentH={2}
                currentW={2}
            >
                Price Chart Stuff
            </Widget>
        );
    }
}