import React, { Component } from 'react';
import { Glyphicon } from 'react-bootstrap';

import './Widget.css';

export class Widget extends Component {
    constructor(props) {
        super(props);
        this.handleRemove = this.handleRemove.bind(this);
        this.state = {
            title: props.title,
            minH: props.minH,
            minW: props.minW,
            currentH: props.currentH,
            currentW: props.currentW,
            x: props.x,
            y: props.y
        };
    }

    handleRemove(e) {
        
    }

    render() {
        return (
            <div key={this.state.title} className="widget" data-grid={{ w: this.state.currentW, h: this.state.currentH, x: this.state.x, y: this.state.y, minW: this.state.minW, minH: this.state.minH }}>
                <div className="widgetHandle">
                    <p className="widgetTitle">{this.state.title}</p>
                    <Glyphicon className="closeButton" onClick={this.handleRemove} glyph='remove' />
                </div>
                <div className="widgetBody">
                    {this.props.children}
                </div>
            </div>
        );
    }
}