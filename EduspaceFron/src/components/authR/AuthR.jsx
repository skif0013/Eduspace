import React from 'react'
import { Link } from 'react-router-dom'

const AuthR = () => {
	return (
		<div className='mt-7 flex justify-center'>
			<form action='#' className='flex flex-col gap-6 w-full max-w-md'>
				{/* Name field */}
				<div className='flex flex-col gap-2'>
					<label htmlFor='name' className='text-xl text-white font-normal'>
						Name
					</label>
					<input
						id='name'
						type='text'
						placeholder='Enter your name'
						className='border outline-none py-2 px-14 text-base text-white bg-gray-700 rounded-xl w-full'
					/>
				</div>

				{/* Email field */}
				<div className='flex flex-col gap-2'>
					<label htmlFor='email' className='text-xl text-white font-normal'>
						Email
					</label>
					<input
						id='email'
						type='email'
						placeholder='Enter your email'
						className='border outline-none py-2 px-14 text-base text-white bg-gray-700 rounded-xl w-full'
					/>
				</div>

				{/* Password field */}
				<div className='flex flex-col gap-2'>
					<label htmlFor='password' className='text-xl text-white font-normal'>
						Password
					</label>
					<input
						id='password'
						type='email'
						placeholder='Enter your password'
						className='border outline-none py-2 px-14 text-base text-white bg-gray-700 rounded-xl w-full '
					/>
				</div>

				<div>
					<span className='text-sm text-white flex items-center gap-2'>
						Log In  →  
						<Link to='/signIn' className='text-blue-600'>
							for existing users
						</Link>
					</span>
				</div>

				{/* Submit button */}
				<button
					type='submit'
					className='bg-blue-600 hover:bg-blue-700 text-white py-2 px-4 rounded-xl font-semibold transition-colors'
				>
					Register
				</button>
			</form>
		</div>
	)
}

export default AuthR
