import React, { Component } from 'react';

import GridLayout from 'react-grid-layout';
import 'react-grid-layout/css/styles.css';
import { MarketWatch } from './Widgets/MarketWatch';
import { Navigator } from './Widgets/Navigator';
import { Output } from './Widgets/Output';
import { Chart } from './Widgets/Chart';
import { DepthChart } from './Widgets/DepthChart';
import { RecentTrades } from './Widgets/RecentTrades';
import { Orderbook } from './Widgets/Orderbook';

export class Home extends Component {
    displayName = Home.name

    render() {
        // layout is an array of objects
        var layout = [
            { i: 'marketWatch', x: 0, y: 0, w: 7, h: 11, minW: 3 },
            { i: 'navigator', x: 0, y: 12, w: 7, h: 11, minH: 10, minW: 4 },
            { i: 'chart', x: 7, y: 0, w: 6, h: 11, minH: 6, minW: 6 },
            { i: 'depth', x: 7, y: 12, w: 6, h: 11, minH: 6, minW: 6 },
            { i: 'recent', x: 13, y: 0, w: 6, h: 11, minH: 6, minW: 6 },
            { i: 'book', x: 13, y: 12, w: 6, h: 11, minH: 6, minW: 6 },
            { i: 'output', x: 11, y: 20, w: 24, h: 12, minH: 6 }
        ];

        return (
            <GridLayout layout={layout} cols={24} rowHeight={7} width={1400} draggableHandle=".widgetHandle">
                <div className="widget" key="marketWatch">
                    <MarketWatch />
                </div>
                <div className="widget" key="navigator">
                    <Navigator />
                </div>
                <div className="widget" key="chart">
                    <Chart />
                </div>
                <div className="widget" key="depth">
                    <DepthChart />
                </div>
                <div className="widget" key="book">
                    <Orderbook />
                </div>
                <div className="widget" key="recent">
                    <RecentTrades />
                </div>
                <div className="widget" key="output">
                    <Output />
                </div>
            </GridLayout>
        );
    }
}
