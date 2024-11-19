import "@mantine/core/styles.css";
import { Routes, Route, BrowserRouter } from "react-router-dom";
import {
	AppShell,
	MantineProvider,
	Burger,
	Group,
	Skeleton,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { theme } from "./theme";
import { HomePage } from "./pages/Home.page";
import MediaPage from "./pages/Media.page";
import MediaDetailsPage from "./pages/MediaDetails.page";

import { Provider, Client, cacheExchange, fetchExchange } from "urql";
import NavBar from "./Navbar";
import LabPage from "./pages/Lab.page";

const client = new Client({
	url: "/graphql",
	exchanges: [cacheExchange, fetchExchange],
	requestPolicy: "cache-first",
});

export default function App() {
	const [opened, { toggle }] = useDisclosure();
	return (
		<Provider value={client}>
			<MantineProvider theme={theme}>
				<AppShell
					header={{ height: 0 }}
					navbar={{
						width: 80,
						breakpoint: "sm",
						collapsed: { mobile: !opened },
					}}
					padding="md"
				>
					<AppShell.Navbar p="xs">
						<NavBar />
					</AppShell.Navbar>
					<AppShell.Main>
						<BrowserRouter>
							<Routes>
								<Route path="/" element={<HomePage />} />
								<Route path="media" element={<MediaPage />}>
									<Route path=":id" element={<MediaDetailsPage />} />
								</Route>

								<Route path="lab" element={<LabPage />} />
							</Routes>
						</BrowserRouter>
					</AppShell.Main>
				</AppShell>
			</MantineProvider>
		</Provider>
	);
}
