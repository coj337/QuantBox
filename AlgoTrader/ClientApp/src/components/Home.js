import React, { Component } from 'react';
import RGL, { WidthProvider } from "react-grid-layout";import 'react-grid-layout/css/styles.css';
import _ from "lodash";

import { MarketWatch } from './Widgets/MarketWatch';
import { Navigator } from './Widgets/Navigator';
import { PriceChart } from './Widgets/PriceChart';
import { DepthChart } from './Widgets/DepthChart';
import { RecentTrades } from './Widgets/RecentTrades';
import { Orderbook } from './Widgets/Orderbook';
import { Output } from './Widgets/Output';

const ReactGridLayout = WidthProvider(RGL);
const originalLayout = getFromLS("layout") || [];
const originalWidgets = getFromLS("widgets") || [
    MarketWatch,
    Navigator,
    PriceChart,
    DepthChart,
    RecentTrades,
    Orderbook,
    Output
];

export class Home extends Component {
    static defaultProps = {
        className: "gridLayout",
        cols: 12,
        rowHeight: 30,
        onLayoutChange: function () { }
    };

    displayName = Home.name

    constructor(props) {
        super(props);

        this.state = {
            layout: JSON.parse(JSON.stringify(originalLayout)),
            widgets: originalWidgets
        };

        this.onAddWidget = this.onAddWidget.bind(this);
        this.onLayoutChange = this.onLayoutChange.bind(this);
        this.createWidget = this.createWidget.bind(this);
    }

    resetLayout() {
        this.setState({
            layout: []
        });
    }

    onLayoutChange(layout) {
        saveToLS("layout", layout);
        this.setState({ layout });
    }

    onAddWidget(name) {
        this.setState({
            layout: this.state.layout.concat({
                i: name,
                x: this.state.widgets.length * 2 % (this.state.cols || 24),
                y: Infinity, // puts it at the bottom
                w: 2,
                h: 2
            })
        });
    }

    onRemoveWidget(name) {
        this.setState({ layout: _.reject(this.state.layout, { name: name }) });
    }

    createWidget(WidgetType) {
        return <WidgetType key={WidgetType.name} />;
    }

    render() {
        return (
            <ReactGridLayout
                {...this.props}
                layout={this.state.layout}
                onLayoutChange={this.onLayoutChange}
                draggableHandle=".widgetHandle"
            >
                {this.state.widgets.map(x => this.createWidget(x))}
            </ReactGridLayout>
        );
    }
}

function getFromLS(key) {
    let ls = {};
    if (global.localStorage) {
        try {
            ls = JSON.parse(global.localStorage.getItem("dashboard")) || {};
        } catch (e) {
            //Ignore
        }
    }
    return ls[key];
}

function saveToLS(key, value) {
    if (global.localStorage) {
        global.localStorage.setItem("dashboard",
            JSON.stringify({
                [key]: value
            })
        );
    }
}