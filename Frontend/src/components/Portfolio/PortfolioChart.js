import React, { Component } from 'react';
import { ResponsiveLine } from 'nivo'
import './PortfolioChart.css';

export class PortfolioChart extends Component {
    displayName = PortfolioChart.name

    constructor(props) {
        super(props);

        this.state = {
            assets: []
        };
    }

    render() {
        const data = [
            {
                "id": "Bitcoin",
                "color": "hsl(355, 70%, 50%)",
                "data": [
                    {
                        "x": "January",
                        "y": 86
                    },
                    {
                        "x": "February",
                        "y": 215
                    },
                    {
                        "x": "March",
                        "y": 132
                    },
                    {
                        "x": "April",
                        "y": 223
                    },
                    {
                        "x": "May",
                        "y": 93
                    },
                    {
                        "x": "June",
                        "y": 39
                    },
                    {
                        "x": "July",
                        "y": 32
                    },
                    {
                        "x": "August",
                        "y": 214
                    },
                    {
                        "x": "September",
                        "y": 296
                    },
                    {
                        "x": "October",
                        "y": 26
                    }
                ]
            },
            {
                "id": "Ethereum",
                "color": "hsl(58, 70%, 50%)",
                "data": [
                    {
                        "x": "January",
                        "y": 2
                    },
                    {
                        "x": "February",
                        "y": 54
                    },
                    {
                        "x": "March",
                        "y": 57
                    },
                    {
                        "x": "April",
                        "y": 229
                    },
                    {
                        "x": "May",
                        "y": 144
                    },
                    {
                        "x": "June",
                        "y": 223
                    },
                    {
                        "x": "July",
                        "y": 55
                    },
                    {
                        "x": "August",
                        "y": 278
                    },
                    {
                        "x": "September",
                        "y": 5
                    },
                    {
                        "x": "October",
                        "y": 53
                    }
                ]
            },
            {
                "id": "Ripple",
                "color": "hsl(86, 70%, 50%)",
                "data": [
                    {
                        "x": "January",
                        "y": 283
                    },
                    {
                        "x": "February",
                        "y": 50
                    },
                    {
                        "x": "March",
                        "y": 292
                    },
                    {
                        "x": "April",
                        "y": 124
                    },
                    {
                        "x": "May",
                        "y": 91
                    },
                    {
                        "x": "June",
                        "y": 284
                    },
                    {
                        "x": "July",
                        "y": 86
                    },
                    {
                        "x": "August",
                        "y": 1
                    },
                    {
                        "x": "September",
                        "y": 202
                    },
                    {
                        "x": "October",
                        "y": 214
                    }
                ]
            },
            {
                "id": "Litecoin",
                "color": "hsl(128, 70%, 50%)",
                "data": [
                    {
                        "x": "January",
                        "y": 192
                    },
                    {
                        "x": "February",
                        "y": 111
                    },
                    {
                        "x": "March",
                        "y": 164
                    },
                    {
                        "x": "April",
                        "y": 111
                    },
                    {
                        "x": "May",
                        "y": 69
                    },
                    {
                        "x": "June",
                        "y": 9
                    },
                    {
                        "x": "July",
                        "y": 199
                    },
                    {
                        "x": "August",
                        "y": 118
                    },
                    {
                        "x": "September",
                        "y": 95
                    },
                    {
                        "x": "October",
                        "y": 186
                    }
                ]
            },
            {
                "id": "Stellar",
                "color": "hsl(42, 70%, 50%)",
                "data": [
                    {
                        "x": "January",
                        "y": 110
                    },
                    {
                        "x": "February",
                        "y": 12
                    },
                    {
                        "x": "March",
                        "y": 172
                    },
                    {
                        "x": "April",
                        "y": 66
                    },
                    {
                        "x": "May",
                        "y": 131
                    },
                    {
                        "x": "June",
                        "y": 148
                    },
                    {
                        "x": "July",
                        "y": 254
                    },
                    {
                        "x": "August",
                        "y": 168
                    },
                    {
                        "x": "September",
                        "y": 61
                    },
                    {
                        "x": "October",
                        "y": 92
                    }
                ]
            }
        ];

        return (
            <div className="chartContainer darkerContainer">
                <ResponsiveLine
                    data={data}
                    margin={{
                        "top": 50,
                        "right": 40,
                        "bottom": 50,
                        "left":60
                    }}
                    enableArea={true}
                    enableGridX={false}
                    enableGridY={false}
                    enableDots={false}
                    animate
                    yScale={{ type: 'linear', stacked: true }}

                />
            </div>
        );
    }
}
