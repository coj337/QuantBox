import React, { Component } from 'react';
import { WidthProvider, Responsive } from "react-grid-layout";
import 'react-grid-layout/css/styles.css';
import _ from "lodash";

import { MarketWatch } from './Widgets/MarketWatch';
import { Navigator } from './Widgets/Navigator';
import { PriceChart } from './Widgets/PriceChart';
import { DepthChart } from './Widgets/DepthChart';
import { RecentTrades } from './Widgets/RecentTrades';
import { Orderbook } from './Widgets/Orderbook';
import { Output } from './Widgets/Output';

const ResponsiveReactGridLayout = WidthProvider(Responsive);
const originalLayouts = getFromLS("layouts") || {};
const originalWidgets = getFromLS("widgets") || [
    { type: MarketWatch },
    { type: Navigator },
    { type: PriceChart },
    { type: DepthChart },
    { type: RecentTrades },
    { type: Orderbook },
    { type: Output }
];

export class Home extends Component {
    displayName = Home.name

    constructor(props) {
        super(props);

        this.state = {
            layouts: originalLayouts,
            widgets: originalWidgets
        };

        this.onAddWidget = this.onAddWidget.bind(this);
        this.onLayoutChange = this.onLayoutChange.bind(this);
        this.createWidget = this.createWidget.bind(this);
    }

    resetLayout() {
        this.setState({ layouts: {} });
    }

    onLayoutChange(layouts) {
        saveToLS("layouts", layouts);
        this.setState({ layouts });
    }

    onAddWidget(name) {
        this.setState({
            layouts: this.state.layouts.concat({
                i: name,
                x: this.state.widgets.length * 2 % (this.state.cols || 24),
                y: Infinity, // puts it at the bottom
                w: 2,
                h: 2
            })
        });
    }

    onRemoveWidget(name) {
        this.setState({ layouts: _.reject(this.state.layouts, { name: name }) });
    }

    createWidget(WidgetType) {
        return <WidgetType key={WidgetType.name}/>;
    }

    render() {
        return (
            <ResponsiveReactGridLayout
                layouts={this.state.layouts}
                onLayoutChange={(layouts) =>
                    this.onLayoutChange(layouts)
                }
                onBreakpointChange={this.onBreakpointChange}
                draggableHandle=".widgetHandle"
            >
                {this.state.widgets.map(x => this.createWidget(x.type))} 
            </ResponsiveReactGridLayout>
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