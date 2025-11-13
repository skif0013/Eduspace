import React, { useState } from 'react'

const Header = () => {
	const [user, setUser] = useState(true)

	const headerPage = [
		{
			id: 1,
			title: 'Home',
			href: '/',
		},
		{
			id: 2,
			title: 'About',
			href: '/about',
		},
		{
			id: 3,
			title: 'Courses',
			href: '/courses',
		},
		{
			id: 4,
			title: 'Blog',
			href: '/blog',
		},
		{
			id: 5,
			title: 'Contact',
			href: '/contact',
		},
	]

	return (
		<div>
			<div className='flex items-center justify-between py-4 px-6'>
				<div>
					<h2 className='text-2xl font-bold text-white'>Eduspace</h2>
				</div>
				<ul className='flex gap-6'>
					{headerPage.map(item => (
						<li key={item.id}>
							<a href={item.href} className='text-base font-normal text-white transition-all duration-300 ease-in-out hover:text-blue-800 '>
								{item.title}
							</a>
						</li>
					))}
				</ul>
				<div className='flex gap-6 items-center'>
					<img
						width={30}
						height={30}
						src='/user.png'
						alt=''
						onClick={prev => prev(setUser)}
					/>
					<button
						className='py-3 px-12 rounded-2xl text-white border border-blue-900
						transition-all duration-300 ease-in-out
						hover:bg-blue-900 hover:border-blue-950 hover:shadow-lg hover:shadow-blue-500/30'
					>
						{user ? 'Registration' : 'Login'}
					</button>
				</div>
			</div>
		</div>
	)
}

export default Header
