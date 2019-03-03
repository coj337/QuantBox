import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Glyphicon, Nav, Navbar, NavItem } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faMeh, faChartPie, faRobot } from '@fortawesome/free-solid-svg-icons'
import { LinkContainer } from 'react-router-bootstrap';
import './NavMenu.css';

export class NavMenu extends Component {
    displayName = NavMenu.name

    render() {
        return (
            <Navbar inverse fixedTop fluid collapseOnSelect>
                <Navbar.Header>
                    <Navbar.Brand>
                        <Link to={'/'}>QuantBox</Link>
                    </Navbar.Brand>
                    <Navbar.Toggle />
                </Navbar.Header>
                <Navbar.Collapse>
                    <Nav>
                        <LinkContainer to={'/'} exact>
                            <NavItem>
                                <Glyphicon glyph='home' /> Dashboard
                            </NavItem>
                        </LinkContainer>
                        <LinkContainer to={'/bots'}>
                            <NavItem>
                                <FontAwesomeIcon icon={faRobot} /> Bots
                            </NavItem>
                        </LinkContainer>
                        <LinkContainer to={'/sentiment'}>
                            <NavItem>
                                <FontAwesomeIcon icon={faMeh} /> Sentiment
                            </NavItem>
                        </LinkContainer>
                        <LinkContainer to={'/arbitrage/normal'}>
                            <NavItem>
                                <Glyphicon glyph='random' /> Normal Arbitrage
                            </NavItem>
                        </LinkContainer>
                        <LinkContainer to={'/arbitrage/triangle'}>
                            <NavItem>
                                <Glyphicon glyph='random' /> Triangle Arbitrage
                            </NavItem>
                        </LinkContainer>
                        <LinkContainer to={'/portfolio'}>
                            <NavItem>
                                <FontAwesomeIcon icon={faChartPie} /> Portfolio
                            </NavItem>
                        </LinkContainer>
                        <LinkContainer to={'/settings'}>
                            <NavItem>
                                <Glyphicon glyph='cog' /> Settings
                            </NavItem>
                        </LinkContainer>
                    </Nav>
                </Navbar.Collapse>
            </Navbar>
        );
    }
}
