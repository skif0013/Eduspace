import React from 'react'
import Header from '../../components/header/Header'
import Slider from '../../components/slider/Slider'
import Block from '../../components/block/Block'
import UsersBlockStatic from '../../components/usersBlock/UsersBlockStatic'
import TitleText from '../../components/titleText/TitleText'
import Mess from '../../components/mess/Mess'
import Footer from '../../components/footer/Footer'

const Home = () => {
	return (
		<div className='bg-gray-900 min-h-screen'>
			<div className='max-w-[1440px] m-auto'>
				<Header />
				<Slider />
				<Block/>
				<TitleText/>
				<UsersBlockStatic/>
				<Mess />
				<Footer />
			</div>
		</div>
	)
}

export default Home
