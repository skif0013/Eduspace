import { Routes, Route } from 'react-router-dom'

import AppContext from './Context/AppContext'

import Home from './pages/home/Home'
import NotFound from './pages/notFound/NotFound'
import SignIn from './pages/signIn/SignIn'
import Register from './pages/register/Register'
import Verfy from './pages/verfy/Verfy'

function App() {
	return (
		<AppContext.Provider value={AppContext}>
			<Routes>
				<Route path='/' element={<Home />} />
				<Route path='*' element={<NotFound />} />
				<Route path='/register' element={<Register />} />
				<Route path='/signIn' element={<SignIn />}>
					<Route path='verfy' element={<Verfy />} />
					{/* <Route path='sailc' element={<Sailc />} /> */}
				</Route>
			</Routes>
		</AppContext.Provider>
	)
}

export default App
