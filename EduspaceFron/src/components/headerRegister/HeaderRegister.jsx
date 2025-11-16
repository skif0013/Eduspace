import React from 'react'
import { Link } from 'react-router-dom'

const HeaderRegister = ({ register }) => {
	return (
		<div>
			<div className='flex flex-col items-center gap-4'>
				<Link to='/' className='flex items-center flex-col gap-4'>
					<img width={65} height={65} src='/user.png' alt='' />
					<h2 className='text-4xl text-white font-bold'>Eduspace</h2>
				</Link>
				<p className='text-xl text-white font-normal w-[400px] text-center'>
					Create your account to start learning and exploring new opportunities.
				</p>

				<h3 className='text-white text-2xl font-semibold '>{register}</h3>
			</div>
		</div>
	)
}

export default HeaderRegister
