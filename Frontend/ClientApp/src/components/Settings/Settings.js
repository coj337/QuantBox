import React, { Component } from 'react';
import { Col, Row } from 'react-bootstrap';
import { SentimentPanel } from './SentimentPanel';

export class Settings extends Component {
    displayName = Settings.name

    constructor(props) {
        super(props);

        this.state = {

        };
    }

    componentDidMount() {
        
    }

    render() {
        return (
            <Row>
                <Col xs={6} md={4} lg={3}>
                    Test
                </Col>
            </Row>
        );
    }
}