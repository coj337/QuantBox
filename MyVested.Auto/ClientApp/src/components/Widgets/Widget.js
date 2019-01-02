import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { Glyphicon } from 'react-bootstrap';

import { PriceChart } from './PriceChart';
import { DepthChart } from './DepthChart';
import { MarketWatch } from './MarketWatch';
import { Navigator } from './Navigator';
import { Output } from './Output';
import { RecentTrades } from './RecentTrades';
import { Orderbook } from './Orderbook';

import './Widget.css';

export class Widget extends Component {
    constructor(props) {
        super(props);
        this.handleRemove = this.handleRemove.bind(this);
        this.state = { type: props.type };
    }

    handleRemove(e) {
        
    }

    render() {
        var body, minH, minW, defaultH, defaultW, title;

        //Assign the right widget
        if (this.state.type === types.PriceChart) {
            minH = 2;
            minW = 2;
            defaultH = 4;
            defaultW = 8;
            title = "Price Chart (Symbol)";
            body = <PriceChart />;
        }
        else if (this.state.type === types.DepthChart) {
            minH = 2;
            minW = 2;
            defaultH = 4;
            defaultW = 4;
            title = "Market Depth (Symbol)";
            body = <DepthChart />;
        }
        else if (this.state.type === types.MarketWatch) {
            minH = 2;
            minW = 2;
            defaultH = 4;
            defaultW = 8;
            title = "Market Watch";
            body = <MarketWatch />;
        }
        else if (this.state.type === types.Navigator) {
            minH = 2;
            minW = 2;
            defaultH = 4;
            defaultW = 4;
            title = "Navigator";
            body = <Navigator />;
        }
        else if (this.state.type === types.Orderbook) {
            minH = 2;
            minW = 2;
            defaultH = 4;
            defaultW = 4;
            title = "Orderbook (Symbol)";
            body = <Orderbook />;
        }
        else if (this.state.type === types.Output) {
            minH = 2;
            minW = 2;
            defaultH = 4;
            defaultW = 4;
            title = "Output";
            body = <Output />;
        }
        else if (this.state.type === types.RecentTrades) {
            minH = 2;
            minW = 2;
            defaultH = 4;
            defaultW = 4;
            title = "Recent Trades (Symbol)";
            body = <RecentTrades />;
        }

        return (
            <span>
                <div className="widgetHandle">
                    <p className="widgetTitle">{title}</p>
                    <Glyphicon className="closeButton" onClick={this.handleRemove} glyph='remove' />
                </div>
                <div className="widgetBody">
                    {body}
                </div>
            </span>
        );
    }
}

//Define an "enum" for supported widget types
const types = {
    PriceChart: 'PriceChart',
    DepthChart: 'DepthChart',
    MarketWatch: 'MarketWatch',
    Navigator: 'Navigator',
    Orderbook: 'Orderbook',
    Output: 'Output',
    RecentTrades: 'RecentTrades'
};
Widget.propTypes = {
    types: PropTypes.oneOf(Object.keys(types))
};
Widget.types = types;