import React, { Component } from 'react';
import { WidthProvider, Responsive } from "react-grid-layout";
import 'react-grid-layout/css/styles.css';
import _ from "lodash";

import { Widget, types } from './Widgets/Widget';
import { Output } from './Widgets/Output';
const ResponsiveReactGridLayout = WidthProvider(Responsive);

export class Home extends Component {
    static defaultProps = {
        className: "layout",
        breakpoints: { lg: 1200, md: 996, sm: 768, xs: 480, xxs: 0 },
        cols: { lg: 12, md: 10, sm: 6, xs: 4, xxs: 2 },
        layouts: { [key: $Keys<breakpoints>]: Layout},
        rowHeight: 100,
        widgetLayouts: [
            { i: Widget.types.MarketWatch, x: 0, y: 0, w: 2, h: 11, minW: 3 },
            { i: Widget.types.Navigator, x: 0, y: 12, w: 7, h: 11, minH: 10, minW: 4 },
            { i: Widget.types.PriceChart, x: 7, y: 0, w: 6, h: 11, minH: 6, minW: 6 },
            { i: Widget.types.DepthChart, x: 7, y: 12, w: 6, h: 11, minH: 6, minW: 6 },
            { i: Widget.types.RecentTrades, x: 13, y: 0, w: 6, h: 11, minH: 6, minW: 6 },
            { i: Widget.types.Orderbook, x: 13, y: 12, w: 6, h: 11, minH: 6, minW: 6 },
            { i: Widget.types.Output, x: 11, y: 20, w: 24, h: 12, minH: 6 }
        ]
    };

    displayName = Home.name

    constructor(props) {
        super(props);
        
        this.state = {
            widgetLayouts: props.widgetLayouts
        };
        
        this.onAddWidget = this.onAddWidget.bind(this);
        this.onBreakpointChange = this.onBreakpointChange.bind(this);
        this.onLayoutChange = this.onLayoutChange.bind(this);
    }

    // We're using the cols coming back from this to calculate where to add new items.
    onBreakpointChange(breakpoint, cols) {
        this.setState({
            breakpoint: breakpoint,
            cols: cols
        });
    }

    onLayoutChange(layout) {
        this.setState({ widgetLayouts: layout });
    }

    createElement(type) {
        return (
            <div className="widget" key={type}>
                <Widget type={type} />
            </div>
        );
    }

    onAddWidget(name) {
        this.setState({
            // Add a new item
            widgetLayouts: this.state.widgetLayouts.concat({
                i: name,
                x: this.state.widgetLayouts.length * 2 % (this.state.cols || 24),
                y: Infinity, // puts it at the bottom
                w: 2,
                h: 2
            })
        });
    }

    onRemoveWidget(name) {
        this.setState({ widgetLayouts: _.reject(this.state.widgetLayouts, { name: name.i }) });
    }

    render() {
        return (
            <ResponsiveReactGridLayout
                onLayoutChange={this.onLayoutChange}
                onBreakpointChange={this.onBreakpointChange}
                draggableHandle=".widgetHandle"
            >
                { this.state.widgetLayouts.map(widget => this.createElement(widget.i))}
            </ResponsiveReactGridLayout>
        );
    }
}
