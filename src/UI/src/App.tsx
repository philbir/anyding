import '@mantine/core/styles.css';

import { AppShell, MantineProvider, Burger, Group, Skeleton } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { Router } from './Router';
import { theme } from './theme';

import { Provider, Client, cacheExchange, fetchExchange } from 'urql'
import NavBar from './Navbar';

const client = new Client({
  url: '/graphql',
  exchanges: [cacheExchange, fetchExchange],
  requestPolicy: 'network-only'
})

export default function App() {
  const [opened, { toggle }] = useDisclosure();
  return (
    <Provider value={client}>
      <MantineProvider theme={theme}>
        <AppShell
          header={{ height: 0 }}
          navbar={{ width: 80, breakpoint: 'sm', collapsed: { mobile: !opened } }}
          padding="md"
        >
          <AppShell.Navbar p="xs">
            <NavBar />
          </AppShell.Navbar>
          <AppShell.Main>
            <Router />
          </AppShell.Main>
        </AppShell>

      </MantineProvider>
    </Provider>
  );
}
