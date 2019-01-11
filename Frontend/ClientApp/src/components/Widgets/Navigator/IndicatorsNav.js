import React, { Component } from 'react';

export class IndicatorsNav extends Component {
    constructor(props) {
        super(props);
        this.state = {
            error: null,
            isLoaded: false,
            indicators: []
        };
    }

    componentDidMount() {
        fetch("/Nav/Indicators")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        isLoaded: true,
                        indicators: result
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
        const { error, isLoaded, indicators } = this.state;
        const indicatorsList = indicators.map((indicator) =>
            <li>{indicator.name}</li>
        );

        if (error) {
            return <div>Error: {error.message}</div>;
        } else if (!isLoaded) {
            return <div>Loading...</div>;
        } else {
            return (
                <ul>{indicatorsList}</ul>
            );
        }
    }
}