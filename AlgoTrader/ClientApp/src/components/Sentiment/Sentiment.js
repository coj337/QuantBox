import React, { Component } from 'react';
import { Col, Row } from 'react-bootstrap';
import { HubConnectionBuilder } from '@aspnet/signalr';
import './Sentiment.css';

export class Sentiment extends Component {
  displayName = Sentiment.name

    constructor(props) {
        super(props);

        this.state = {
            hubConnection: null,
            sentiments: []
        };
    }

    componentWillMount() {
        const hubConnection = new HubConnectionBuilder().withUrl('/sentimentHub').build();
        this.setState({
            hubConnection: hubConnection
        }, () => {
            this.state.hubConnection
                .start()
                .then(() => console.log('Connection started!'))
                .catch(err => console.log('Error while establishing connection :( (' + err + ')'));
        });
    }

    componentDidMount() {
        this.state.hubConnection.on('ReceiveSentiments', (receivedMessage) => {
            var sentiments = receivedMessage;
            this.setState({
                sentiments: sentiments
            });
        });
    }

    render() {
        return (
            <Row>
                {this.state.sentiments.map((sentiment, i) => (
                    <Col xs={6} md={4} lg={3} key={i}>
                        <div className="sentimentPanel" >
                            Asset: {sentiment.name} ({sentiment.symbol})<br/>
                            Positive: {Math.floor(sentiment.positive * 100)}<br />
                            Neutral: {Math.floor(sentiment.neutral * 100)}<br />
                            Negative: {Math.floor(sentiment.negative * 100)}<br />
                            Compound: {Math.floor(sentiment.compound * 100)}<br />
                            Tweets Analyzed: {sentiment.itemsChecked}
                        </div>
                    </Col>
                ))}
            </Row>
        );
    }
}
