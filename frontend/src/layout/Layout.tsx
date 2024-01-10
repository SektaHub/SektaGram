import React, { ReactNode } from 'react';
import Navbar from '../components/Navbar/Navbar';

interface LayoutProps {
  children: ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  return (
    <div className="layout">
      <Navbar />
      <main>{children}</main>
    </div>
  );
};

export default Layout;
