import React from 'react'
import {
	Bot,
	Settings,
	ShieldCheck,
	CreditCard,
	Gamepad2,
	Headphones,
} from 'lucide-react'

const iconsMap = {
	bot: Bot,
	settings: Settings,
	'shield-check': ShieldCheck,
	'credit-card': CreditCard,
	gamepad: Gamepad2,
	support: Headphones,
}

const Block = () => {
const data = [
  {
    id: 1,
    icons: 'bot',
    title: 'AI-Powered Answers',
    text: 'Get clear and accurate answers to your study questions using artificial intelligence.',
  },
  {
    id: 2,
    icons: 'settings',
    title: 'Query Customization',
    text: 'Adjust the topic, difficulty level, and response format to fit your learning needs.',
  },
  {
    id: 3,
    icons: 'shield-check',
    title: 'Privacy & Security',
    text: 'All questions are processed securely and are not stored in the system.',
  },
  {
    id: 4,
    icons: 'credit-card',
    title: 'Feature Access',
    text: 'Flexible access to platform features for learning, practice, and research.',
  },
  {
    id: 5,
    icons: 'gamepad',
    title: 'Examples & Practice',
    text: 'Step-by-step explanations and practical examples to improve understanding.',
  },
  {
    id: 6,
    icons: 'support',
    title: 'User Support',
    text: 'Assistance with platform usage and answers to user-related questions.',
  },
]


	return (
		<div className='m-auto flex flex-col items-center justify-center gap-8'>
      <div>
        <h2 className='text-3xl text-white font-bold'>What We Offer?</h2>
      </div>
			<div className='max-w-[1900px] flex m-auto items-center flex-wrap gap-4 justify-center'>
				{data.map(item => {
					const Icon = iconsMap[item.icons]

					return (
						<div key={item.id} className='max-w-[450px] cursor-pointer border border-blue-900 bg-gray-100/5 border-0.5 h-[250px] justify-center py-4 px-8 rounded-4xl items-center flex flex-col text-center gap-4'>
							<Icon size={55} className='text-white/30' />
							<h2 className='text-white text-2xl font-semibold'>{item.title}</h2>
							<p className='text-white text-base'>{item.text}</p>
						</div>
					)
				})}
			</div>
		</div>
	)
}

export default Block
