import React, { Component } from 'react';
import './Sentiment.css';

export class SentimentPanel extends Component {
    displayName = SentimentPanel.name

    constructor(props) {
        super(props);

        this.state = {
            error: null,
            isLoaded: false,
            name: props.name,
            symbol: props.symbol,
            sentiment: null
        };
    }

    componentDidMount() {
        fetch("/Sentiment/GetTwitterSentiment?name=" + this.state.name + "&symbol=" + this.state.symbol)
            .then(res => res.json())
            .then(
                (result) => {
                    console.log(result);
                    this.setState({
                        isLoaded: true,
                        sentiment: result
                    });
                },
                (error) => {
                    this.setState({
                        isLoaded: true,
                        error
                    });
                }
            );
    }

    render() {
        const { error, isLoaded, name, symbol, sentiment } = this.state;

        if (error) {
            return <div>Error: {error.message}</div>;
        } else if (!isLoaded) {
            return <div>Loading...</div>;
        } else {
            return (
                <div className="sentimentPanel">
                    Asset: {name} ({symbol})<br />
                    Positive: {Math.floor(sentiment.positive * 100)}<br />
                    Neutral: {Math.floor(sentiment.neutral * 100)}<br />
                    Negative: {Math.floor(sentiment.negative * 100)}<br />
                    Compound: {Math.floor(sentiment.compound * 100)}<br />
                    Tweets Analyzed: {sentiment.itemsChecked}
                </div>
            );
        }
    }
}