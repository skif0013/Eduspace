Return result

Event Publishing

Redis Pub/Sub is used to publish domain events:

course.created

course.published

course.archived

IMessagePublisher abstraction is implemented by RedisMessagePublisher.

Distributed Cache Implementation