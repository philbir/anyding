import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { HomePage } from './pages/Home.page';
import { MediaPage } from './pages/Media.page';

const router = createBrowserRouter([
  {
    path: '/',
    element: <HomePage />,
  },
  {
    path: 'media',
    element: <MediaPage />,
  },
]);

export function Router() {
  return <RouterProvider router={router} />;
}
