import React, { Component } from 'react';
import { Row } from 'react-bootstrap';
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import "react-tabs/style/react-tabs.css";
import { TemplatePreview } from './TemplatePreview';

export class TemplateChooser extends Component {
    displayName = TemplateChooser.name

    constructor(props) {
        super(props);

        this.state = {
            templates: []
        };
    }

    render() {
        return (
            <Row>
                <Row>
                    <h3>Bot Templates</h3>
                </Row>

                <Tabs>
                    <TabList>
                        <Tab>Your Templates</Tab>
                        <Tab>Templates Market</Tab>
                    </TabList>

                    <TabPanel>
                        <TemplatePreview
                            name="Blank Template"
                            description="Start with a blank bot and configure it yourself."
                        />
                        {this.state.templates.map((template) => (
                            <TemplatePreview
                                name={template.name}
                                description={template.description}
                            />
                        ))}
                    </TabPanel>
                    <TabPanel>
                        Coming Soon
                    </TabPanel>
                </Tabs>
            </Row>
        );
    }
}