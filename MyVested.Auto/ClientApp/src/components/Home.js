import React, { Component } from 'react';

import GridLayout from 'react-grid-layout';
import 'react-grid-layout/css/styles.css';
import { MarketWatch } from './Widgets/MarketWatch';
import { Navigator } from './Widgets/Navigator';
import { Output } from './Widgets/Output';

export class Home extends Component {
    displayName = Home.name

    render() {
        // layout is an array of objects
        var layout = [
            { i: 'a', x: 0, y: 0, w: 7, h: 11, minW: 3 },
            { i: 'b', x: 7, y: 0, w: 4, h: 11, minH: 10, minW: 4 },
            { i: 'c', x: 11, y: 0, w: 6, h: 11, minH: 6, minW: 6 },
            { i: 'output', x: 11, y: 12, w: 24, h: 8, minH: 6 }
        ];

        return (
            <GridLayout layout={layout} cols={24} rowHeight={7} width={1200} draggableHandle=".widgetHandle">
                <div className="widget" key="a">
                    <MarketWatch />
                </div>
                <div className="widget" key="b">
                    <Navigator />
                </div>
                <div className="widget" key="c">
                    <div className="widgetHandle">Chart</div>
                    <div className="widgetBody">Body</div>
                </div>
                <div className="widget" key="output">
                    <Output />
                </div>
            </GridLayout>
        );
    }
}
